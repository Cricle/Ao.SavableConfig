using System;
using System.Linq;
using System.Collections.Generic;

namespace Ao.SavableConfig.Saver
{
    /// <summary>
    /// 更改保存器
    /// </summary>
    public class ChangeSaver
    {
        /// <summary>
        /// 初始化类型<see cref="ChangeSaver"/>
        /// </summary>
        /// <param name="reports"></param>
        /// <param name="transfers"></param>
        public ChangeSaver(IEnumerable<ChangeReport> reports, IEnumerable<IChangeTransferCondition> transfers)
        {
            Reports = reports ?? throw new ArgumentNullException(nameof(reports));
            Transfers = transfers ?? throw new ArgumentNullException(nameof(transfers));
        }
        public ChangeSaver(IEnumerable<ChangeReport> reports, params IChangeTransferCondition[] transfers)
        {
            Reports = reports ?? throw new ArgumentNullException(nameof(reports));
            Transfers = transfers ?? throw new ArgumentNullException(nameof(transfers));
        }
        /// <summary>
        /// 报告集合
        /// </summary>
        public IEnumerable<ChangeReport> Reports { get; }
        /// <summary>
        /// 转移条件器
        /// </summary>
        public IEnumerable<IChangeTransferCondition> Transfers { get; }
        /// <summary>
        /// 执行转移
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<ChangeReport, ChangeEmitResult> Emit()
        {
            if (!Transfers.Any())
            {
                return new Dictionary<ChangeReport, ChangeEmitResult>(0);
            }
            var map = new Dictionary<ChangeReport, ChangeEmitResult>();
            foreach (var item in Reports)
            {
                if (item.IncludeChangeInfo.Count == 0)
                {
                    continue;
                }
                foreach (var transfer in Transfers)
                {
                    var trans = transfer.GetTransfe(item);
                    if (trans != null)
                    {
                        var str = trans.Transfe(item);
                        var res = new ChangeEmitResult(item,transfer, trans,str);
                        map.Add(item, res);
                    }
                }
            }
            return map;
        }
    }
}
