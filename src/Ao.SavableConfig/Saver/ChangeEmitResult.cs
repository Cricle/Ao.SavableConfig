namespace Ao.SavableConfig.Saver
{
    public class ChangeEmitResult
    {
        public ChangeEmitResult(ChangeReport report, IChangeTransferCondition selectCondition, IChangeTransfer transfer, string transfed)
        {
            Report = report;
            SelectCondition = selectCondition;
            Transfer = transfer;
            Transfed = transfed;
        }
        public ChangeReport Report { get; }

        public IChangeTransferCondition SelectCondition { get; }

        public IChangeTransfer Transfer { get; }

        public string Transfed { get; }
    }
}
