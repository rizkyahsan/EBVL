using EBVL.BackEnd.Services.EmailBlast2.Model;

namespace EBVL.BackEnd.Services.EmailBlast2;

public interface IEmailBlast2Service
{
    public void SendEmails(SendEmailInput2 input);
}
