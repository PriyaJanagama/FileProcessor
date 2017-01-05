namespace EmailSender
{
    interface IEmailInterface
    {
        void Send(string to, string message);
    }
}
