using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Marfil.Inf.Genericos.Helper
{
    public static class TransactionScopeBuilder
    {
        /// <summary>
        /// Creates a transactionscope with ReadCommitted Isolation, the same level as sql server
        /// </summary>
        /// <returns>A transaction scope</returns>
        public static TransactionScope CreateTransactionObject()
        {
            var option = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.RepeatableRead
            };
            return new TransactionScope(TransactionScopeOption.Required, option);

        }
    }
}
