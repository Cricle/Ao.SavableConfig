
namespace Ao.SavableConfig.Saver
{
    public interface IChangeTransferCondition
    {
        IChangeTransfer GetTransfe(ChangeReport report);

        void Save(ChangeReport report, string transfed);
    }
}
