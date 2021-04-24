using System.Collections.Generic;

namespace Ao.SavableConfig.Saver
{
    public static class ChangeSaverExtensions
    {
        public static IReadOnlyDictionary<ChangeReport, ChangeEmitResult> EmitAndSave(this ChangeSaver saver)
        {
            var vals=saver.Emit();
            foreach (var item in vals)
            {
                if (item.Value.SelectCondition!=null)
                {
                    item.Value.SelectCondition.Save(item.Value.Report,
                        item.Value.Transfed);
                }
            }
            return vals;
        }
    }
}
