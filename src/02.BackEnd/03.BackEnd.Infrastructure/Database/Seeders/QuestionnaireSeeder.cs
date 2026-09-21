using EBVL.Shared.Enums;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class QuestionnaireSeeder(IDatabaseService db)
{
    private static readonly Guid _questionnaireId = Guid.Parse("61000000-0000-0000-0000-000000000001");
    private static Guid Id(int kind, int number)
    {
        return Guid.Parse($"61000000-0000-0000-{kind:0000}-{number:000000000000}");
    }

    public async Task SeedQuestionnaires()
    {
        if (await db.Questionnaires.AnyAsync(x => x.Code == "VENDOR_REGISTRATION" && !x.IsDeleted))
        {
            return;
        }

        var questionnaire = new Questionnaire { Id = _questionnaireId, QuestionnaireSeriesId = _questionnaireId, Code = "VENDOR_REGISTRATION", BusinessProcess = "Vendor Registration", Version = 1, Status = QuestionnaireStatus.Publish, IsActive = true, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = "EBVLSystem" };
        (string Title, string Code, VendorCompanyStatusType? CompanyType, int[] Order)[] definitions =
        [
            (QuestionnaireFor.GeneralInformation, "GENERAL", null, [.. Enumerable.Range(1,13)]),
            (QuestionnaireFor.VendorRepresentativeOffice, "VENDOR_RO", VendorCompanyStatusType.AuthorizedAgent, new[] {14,17,15,18,16,19}),
            (QuestionnaireFor.SoleAgent, "SOLE_AGENT", VendorCompanyStatusType.SoleDistributorAgent, new[] {20,24,21,25,22,26,23}),
            (QuestionnaireFor.ProductQualityGeneral, "QUALITY_GENERAL", null, [27,30,28,31,29,32]),
            (QuestionnaireFor.ProductQualitySpecific, "QUALITY_SPECIFIC", VendorCompanyStatusType.SoleDistributorAgent, new[] {33,39,34,40,35,41,36,42,37,43,38,44}),
            (QuestionnaireFor.ProductPositioning, "PRODUCT_POSITIONING", VendorCompanyStatusType.AuthorizedAgent, new[] {45,48,46,49,47,50})
        ];
        for (var s = 0; s < definitions.Length; s++)
        {
            var (title, code, companyType, questionOrder) = definitions[s];
            var section = new QuestionnaireSection { Id = Id(1, s + 1), QuestionnaireId = _questionnaireId, Code = code, Title = title, Order = s + 1, CompanyType = companyType, IsActive = true };
            for (var q = 0; q < questionOrder.Length; q++)
            {
                var source = QuestionnaireFor.All.Single(x => x.Number == questionOrder[q]);
                section.Questions.Add(new QuestionnaireQuestion { Id = Id(2, source.Number), Code = $"Q{source.Number:000}", Label = source.Label, Hint = source.Hint, Placeholder = source.Placeholder, Type = source.IsFileUpload ? QuestionnaireQuestionType.File : source.Number is 6 or 7 or 8 or 9 ? QuestionnaireQuestionType.Address : QuestionnaireQuestionType.ShortText, Order = q + 1, IsRequired = source.IsRequired, IsVisible = true, IsActive = true, AnswerRule = source.IsRequired ? QuestionnaireAnswerRule.Mandatory : QuestionnaireAnswerRule.Optional });
            }

            questionnaire.Sections.Add(section);
        }

        _ = await db.Questionnaires.AddAsync(questionnaire);
        _ = await db.SaveAsync(nameof(SeedQuestionnaires));
        await EnsureGeneralQuestions(questionnaire);
        await EnsureVendorRepresentativeOfficeQuestions(questionnaire);
        await EnsureSoleAgentQuestions(questionnaire);
        await EnsureProductQualityGeneralQuestions(questionnaire);
        await EnsureProductQualitySpecificQuestions(questionnaire);
        await EnsureProductPositioningQuestions(questionnaire);
    }

    private async Task EnsureGeneralQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "GENERAL");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "GENERAL", Title = "General", Order = 1, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label, QuestionnaireQuestionType Type, QuestionnaireAnswerRule Rule, VendorCompanyStatusType? CompanyType)[] definitions =
        [
            ("BRAND_NAME", "Q001", "Brand Name", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("PRODUCT", "Q002", "Product", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("VENDOR_COMPANY_NAME", "Q003", "Vendor / Company Name", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("COMPANY_ESTABLISHMENT", "Q004", "Company Establishment", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("HEADQUARTER_INFORMATION", "Q006", "Headquarter information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Mandatory, null),
            ("INITIAL_PRODUCTION", "Q005", "Initial Production", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("VENDOR_COMPANY_PROFILE", "Q010", "Vendor Company Profile", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Mandatory, VendorCompanyStatusType.Manufacture),
            ("SOLE_AGENT_COMPANY_PROFILE", "Q012", "Sole Agent Company Profile", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.SoleDistributorAgent),
            ("LATEST_COMPANY_ANNUAL_REPORT", "Q013", "The Latest Company Annual Report", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.AddedValue, null),
            ("MANUFACTURING_INFORMATION", "Q007", "Manufacturing information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Mandatory, VendorCompanyStatusType.Manufacture),
            ("REPRESENTATIVE_OFFICE_INFORMATION", "Q008", "Representative Office information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.AuthorizedAgent),
            ("SOLE_AGENT_OFFICE_INFORMATION", "Q009", "Sole Agent Office information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.SoleDistributorAgent)
        ];

        for (var index = 0; index < definitions.Length; index++)
        {
            var definition = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == definition.Code || x.Code == definition.LegacyCode || x.Label == definition.Label || (definition.Code == "BRAND_NAME" && x.Label == "Name of Brand"));
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = definition.Code, Label = definition.Label };
                section.Questions.Add(question);
            }

            question.Code = definition.Code;
            question.Label = definition.Label;
            question.Type = definition.Type;
            question.Order = index + 1;
            question.IsRequired = definition.Rule == QuestionnaireAnswerRule.Mandatory;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = definition.Rule;
            question.CompanyType = definition.CompanyType;
        }

        var obsoleteRepresentativeProfile = section.Questions.FirstOrDefault(x => x.Code == "Q011");
        _ = obsoleteRepresentativeProfile?.IsDeleted = true;

        _ = await db.SaveAsync(nameof(EnsureGeneralQuestions));
    }

    private async Task EnsureVendorRepresentativeOfficeQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "VENDOR_RO");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "VENDOR_RO", Title = "Vendor / RO (Representative Office)", Order = 2, CompanyType = VendorCompanyStatusType.AuthorizedAgent, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label, QuestionnaireAnswerRule Rule)[] definitions =
        [
            ("BUSINESS_NAME_CARD_DIRECTOR", "Q014", "Business Name Card (Director)", QuestionnaireAnswerRule.Mandatory),
            ("CERTIFICATE_COMPANY_DOMICILE", "Q017", "Certificate of Company Domicile (COD/Surat Keterangan Domisili)", QuestionnaireAnswerRule.Mandatory),
            ("ID_CARD_PASSPORT_AUTHORIZED_SIGNATORIES", "Q015", "ID Card/Passport for authorized Signatories", QuestionnaireAnswerRule.AddedValue),
            ("TAX_IDENTIFICATION_NUMBER_COMPANY", "Q018", "Tax Identification Number (TIN/NPWP)-Company", QuestionnaireAnswerRule.Mandatory),
            ("DEED_COMPANY_ESTABLISHMENT", "Q016", "Deed of Company Establishment (ACTA/ Akta Pendirian Perusahaan)", QuestionnaireAnswerRule.Mandatory),
            ("CERTIFICATE_COMPANY_REGISTRATION", "Q019", "Certificate of Company Registration (TDP)", QuestionnaireAnswerRule.Mandatory)
        ];

        section.Title = "Vendor / RO (Representative Office)";
        section.CompanyType = VendorCompanyStatusType.AuthorizedAgent;
        section.IsActive = true;
        for (var index = 0; index < definitions.Length; index++)
        {
            var (code, legacyCode, label, rule) = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == code || x.Code == legacyCode || x.Label == label);
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = code, Label = label };
                section.Questions.Add(question);
            }

            question.Code = code;
            question.Label = label;
            question.Type = QuestionnaireQuestionType.File;
            question.Order = index + 1;
            question.IsRequired = rule == QuestionnaireAnswerRule.Mandatory;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = rule;
            question.CompanyType = VendorCompanyStatusType.AuthorizedAgent;
        }

        _ = await db.SaveAsync(nameof(EnsureVendorRepresentativeOfficeQuestions));
    }

    private async Task EnsureSoleAgentQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "SOLE_AGENT");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "SOLE_AGENT", Title = "Sole Agent", Order = 3, CompanyType = VendorCompanyStatusType.SoleDistributorAgent, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label)[] definitions =
        [
            ("SOLE_AGENT_BUSINESS_NAME_CARD_DIRECTOR", "Q020", "Business Name Card (Director)"),
            ("SKUP_MIGAS_ESDM", "Q024", "SKUP (Surat Kemampuan Usaha Penunjang) Migas/ESDM (Base on National Regulation)"),
            ("COMPANY_BUSINESS_LICENSES_SIUP", "Q021", "Company Business Licenses (SIUP)"),
            ("SKT_PERTAMINA", "Q025", "SKT (Surat Keterangan Terdaftar) Pertamina"),
            ("STP_TRADE_MINISTRY", "Q022", "STP (Surat Tanda Pendaftaran) Kemendag/ Trade Ministry"),
            ("CSMS_CERTIFICATE_PERTAMINA", "Q026", "CSMS (Contractor Safety Management System) Certificate Pertamina (Only for SA Provide service for product)"),
            ("AGENCY_AGREEMENT_FROM_VENDOR", "Q023", "Agency Agreement from Vendor")
        ];

        section.Title = "Sole Agent";
        section.CompanyType = VendorCompanyStatusType.SoleDistributorAgent;
        section.IsActive = true;
        for (var index = 0; index < definitions.Length; index++)
        {
            var (code, legacyCode, label) = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == code || x.Code == legacyCode || x.Label == label);
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = code, Label = label };
                section.Questions.Add(question);
            }

            question.Code = code;
            question.Label = label;
            question.Type = QuestionnaireQuestionType.File;
            question.Order = index + 1;
            question.IsRequired = true;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = QuestionnaireAnswerRule.Mandatory;
            question.CompanyType = VendorCompanyStatusType.SoleDistributorAgent;
        }

        _ = await db.SaveAsync(nameof(EnsureSoleAgentQuestions));
    }

    private async Task EnsureProductQualityGeneralQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "QUALITY_GENERAL");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "QUALITY_GENERAL", Title = "Product Quality - General", Order = 4, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label)[] definitions =
        [
            ("BRAND_CERTIFICATE", "Q027", "Brand Certificate"),
            ("SAMPLE_COMPONENT_MILL_CERTIFICATE", "Q030", "Sample Component Mill Certificate"),
            ("PATENT_LICENSE_PRODUCT_DESIGN_CERTIFICATE", "Q028", "Patent/License Certificate and/or International Product Design Standard Certificate"),
            ("USER_SATISFACTION_LETTER", "Q031", "User Satisfaction Letter (Testimony)"),
            ("COMPONENT_SUPPLIER_EXPERT_LIST", "Q029", "Component Supplier/Expert List"),
            ("LOCAL_CONTENT_CERTIFICATE_TKDN", "Q032", "Local Content Certificate (TKDN from Kemenperin)")
        ];

        section.Title = "Product Quality - General";
        section.CompanyType = null;
        section.IsActive = true;
        for (var index = 0; index < definitions.Length; index++)
        {
            var (code, legacyCode, label) = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == code || x.Code == legacyCode || x.Label == label);
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = code, Label = label };
                section.Questions.Add(question);
            }

            question.Code = code;
            question.Label = label;
            question.Type = QuestionnaireQuestionType.File;
            question.Order = index + 1;
            question.IsRequired = false;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = QuestionnaireAnswerRule.AddedValue;
            question.CompanyType = null;
        }

        _ = await db.SaveAsync(nameof(EnsureProductQualityGeneralQuestions));
    }

    private async Task EnsureProductQualitySpecificQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "QUALITY_SPECIFIC");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "QUALITY_SPECIFIC", Title = "Product Quality - Spesific", Order = 5, CompanyType = VendorCompanyStatusType.SoleDistributorAgent, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label, QuestionnaireAnswerRule Rule)[] definitions =
        [
            ("LATEST_PRODUCT_CATALOGUE", "Q033", "Latest Product Catalogue", QuestionnaireAnswerRule.Mandatory),
            ("SAMPLE_FAT_REPORT", "Q039", "Sample of FAT Report", QuestionnaireAnswerRule.AddedValue),
            ("LATEST_PRODUCT_EXPERIENCE_LIST", "Q034", "Latest Product Experience List", QuestionnaireAnswerRule.Mandatory),
            ("SAMPLE_CONFORMITY_CERTIFICATE", "Q040", "Sample Conformity Certificate", QuestionnaireAnswerRule.AddedValue),
            ("MANUFACTURING_TYPE_FULLY", "Q035", "Manufacturing Type - Fully (Design & Manufacturer) Packager/ Integrator", QuestionnaireAnswerRule.Mandatory),
            ("SAMPLE_SITE_ACCEPTANCE_TEST_REPORT", "Q041", "Sample of Site Acceptance Test (SAT) Report", QuestionnaireAnswerRule.Optional),
            ("INTERNATIONAL_PRODUCT_MFG_STANDARD_CERT", "Q036", "International Product Manufacturing Standard Certificate", QuestionnaireAnswerRule.Mandatory),
            ("QUALITY_GUARANTEE_LETTER_HQ", "Q042", "Quality Guarantee Letter from HQ", QuestionnaireAnswerRule.Mandatory),
            ("QUALITY_MANAGEMENT_SYSTEM", "Q037", "Quality Management System", QuestionnaireAnswerRule.Mandatory),
            ("PRODUCT_OBSOLESCENCE_LETTER", "Q043", "Product Obsolescence Letter", QuestionnaireAnswerRule.Optional),
            ("SAMPLE_INSPECTION_TEST_PLAN", "Q038", "Sample of Inspection Test Plan", QuestionnaireAnswerRule.Mandatory),
            ("SAMPLE_TASA_PROGRAM", "Q044", "Sample of TASA (Technical Assistance Services Agreement) Program", QuestionnaireAnswerRule.Optional)
        ];

        section.Title = "Product Quality - Spesific";
        section.CompanyType = VendorCompanyStatusType.SoleDistributorAgent;
        section.IsActive = true;
        for (var index = 0; index < definitions.Length; index++)
        {
            var (code, legacyCode, label, rule) = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == code || x.Code == legacyCode || x.Label == label);
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = code, Label = label };
                section.Questions.Add(question);
            }

            question.Code = code;
            question.Label = label;
            question.Type = QuestionnaireQuestionType.File;
            question.Order = index + 1;
            question.IsRequired = rule == QuestionnaireAnswerRule.Mandatory;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = rule;
            question.CompanyType = null;
        }

        _ = await db.SaveAsync(nameof(EnsureProductQualitySpecificQuestions));
    }

    private async Task EnsureProductPositioningQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "PRODUCT_POSITIONING");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "PRODUCT_POSITIONING", Title = "Product Positioning & Technical Support", Order = 6, CompanyType = VendorCompanyStatusType.AuthorizedAgent, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label, QuestionnaireQuestionType Type, QuestionnaireAnswerRule Rule)[] definitions =
        [
            ("MARKET_SHARE_WORLD", "Q045", "Market Share (%) (In the world)", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Optional),
            ("BRAND_PRODUCT_COMPETITOR", "Q048", "Brand Product Competitor", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.AddedValue),
            ("COUNTRY_ORIGIN_FACTORY_LOCATION", "Q046", "Country of Origin/Factory Location", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Mandatory),
            ("PRODUCT_TECHNOLOGY_LEADER_FOLLOWER", "Q049", "Product Technology Leader/Follower", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory),
            ("PRODUCT_REGIONAL_SUPPLY", "Q047", "Product Regional Supply", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Mandatory),
            ("AFTER_SALES_SERVICE_OFFICE_VENDOR", "Q050", "After sales service office/vendor", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.AddedValue)
        ];

        section.Title = "Product Positioning & Technical Support";
        section.CompanyType = VendorCompanyStatusType.AuthorizedAgent;
        section.IsActive = true;
        for (var index = 0; index < definitions.Length; index++)
        {
            var (code, legacyCode, label, type, rule) = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == code || x.Code == legacyCode || x.Label == label);
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = code, Label = label };
                section.Questions.Add(question);
            }

            question.Code = code;
            question.Label = label;
            question.Type = type;
            question.Order = index + 1;
            question.IsRequired = rule == QuestionnaireAnswerRule.Mandatory;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = rule;
            question.CompanyType = null;
        }

        _ = await db.SaveAsync(nameof(EnsureProductPositioningQuestions));
    }
}
