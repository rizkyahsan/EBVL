using EBVL.Shared.Statics.Statuses;

namespace EBVL.BackEnd.Infrastructure.Database.InitialData;

public static class InitialStatuses
{
    #region Project
    public static readonly Status P0 = new()
    {
        Id = new Guid("a75dc4c6-38ad-4200-96e8-19a35951e839"),
        Table = TableFor.Project,
        Name = "Draft",
        Code = "P0",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status P1 = new()
    {
        Id = new Guid("4f8bfe62-bd78-44f4-85ac-158c3c8829b8"),
        Table = TableFor.Project,
        Name = "On Progress",
        Code = "P1",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status P2 = new()
    {
        Id = new Guid("fe9fc0ce-728d-4368-bdbd-d6243e106b03"),
        Table = TableFor.Project,
        Name = "Complete",
        Code = "P2",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status P9 = new()
    {
        Id = new Guid("8cc31143-12b5-4295-b6dc-2b1367cc8a7f"),
        Table = TableFor.Project,
        Name = "Cancel",
        Code = "P9",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };
    #endregion

    #region Project Lender
    public static readonly Status PL0 = new()
    {
        Id = new Guid("b7db3bca-d13d-4353-9c63-e35321f4f2a2"),
        Table = TableFor.ProjectLender,
        Name = "Draft",
        Code = "PL0",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PL1 = new()
    {
        Id = new Guid("b2b7a730-bf1d-4707-8df1-b2df1ae12986"),
        Table = TableFor.ProjectLender,
        Name = "On Progress",
        Code = "PL1",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PL2 = new()
    {
        Id = new Guid("60eb475b-2c8c-4fb7-aa9a-dba679b8dbd5"),
        Table = TableFor.ProjectLender,
        Name = "Win",
        Code = "PL2",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PL3 = new()
    {
        Id = new Guid("f673776f-ccea-4433-98d2-8df02bd9d6e4"),
        Table = TableFor.ProjectLender,
        Name = "Lose",
        Code = "PL3",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PL9 = new()
    {
        Id = new Guid("83048592-8cad-4414-b96d-254b0fc3b72a"),
        Table = TableFor.ProjectLender,
        Name = "Cancel / Delete",
        Code = "PL9",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };
    #endregion

    #region Project Stage
    public static readonly Status PS0 = new()
    {
        Id = new Guid("4a792811-78a2-47f0-8aa3-02141de42360"),
        Table = TableFor.ProjectStage,
        Name = "Draft",
        Code = "PS0",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PS1 = new()
    {
        Id = new Guid("f50f908e-6189-4077-a327-50352fd9d66e"),
        Table = TableFor.ProjectStage,
        Name = "On Progress",
        Code = "PS1",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PS2 = new()
    {
        Id = new Guid("0065bd54-23e4-440b-9bc3-9a8de4fd4b0a"),
        Table = TableFor.ProjectStage,
        Name = "On Review",
        Code = "PS2",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PS3 = new()
    {
        Id = new Guid("624fa119-cb59-4414-b231-068c6682d276"),
        Table = TableFor.ProjectStage,
        Name = "Complete",
        Code = "PS3",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PS9 = new()
    {
        Id = new Guid("83d6e14b-8413-45ad-88b9-0562a4ae2bf2"),
        Table = TableFor.ProjectStage,
        Name = "Cancel / Delete",
        Code = "PS9",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };
    #endregion

    #region Project Lender Requirement
    public static readonly Status PR0 = new()
    {
        Id = new Guid("837ec4a0-5a1c-4c23-bf59-63e2c7fd3a41"),
        Table = TableFor.ProjectLenderReq,
        Name = "Draft",
        Code = "PR0",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR1 = new()
    {
        Id = new Guid("da5de1cd-6933-4d99-bf2f-9732ac1221fe"),
        Table = TableFor.ProjectLenderReq,
        Name = "On Progress",
        Code = "PR1",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR2 = new()
    {
        Id = new Guid("e7b55e8f-7bd2-4041-b0df-af25ec9bbc10"),
        Table = TableFor.ProjectLenderReq,
        Name = "Submit",
        Code = "PR2",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR3 = new()
    {
        Id = new Guid("9395db84-4798-41b6-b0a2-1da9c4d5833a"),
        Table = TableFor.ProjectLenderReq,
        Name = "Revision",
        Code = "PR3",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR4 = new()
    {
        Id = new Guid("f1a9296b-177a-4b97-9dce-a119c4fdf089"),
        Table = TableFor.ProjectLenderReq,
        Name = "Accept",
        Code = "PR4",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR5 = new()
    {
        Id = new Guid("5ff60885-cf56-4180-9fe3-33df15d80149"),
        Table = TableFor.ProjectLenderReq,
        Name = "Reject",
        Code = "PR5",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Status PR9 = new()
    {
        Id = new Guid("c78cb50e-071d-4fe3-ae0f-142645cb5428"),
        Table = TableFor.ProjectLenderReq,
        Name = "Cancel / Delete",
        Code = "PR9",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };
    #endregion

    public static readonly Status[] All =
    [
        P0, P1, P2, P9,
        PL0, PL1, PL2, PL3, PL9,
        PS0, PS1, PS2, PS3, PS9,
        PR0, PR1, PR2, PR3, PR4, PR5, PS9,
    ];
}
