using System;
using System.Collections.Generic;
using System.Text;

namespace LockDemo
{
    public class Account
    {
        private readonly object balanceLock = new object();
        private decimal balance;

        public Account(decimal initialBalance) => balance = initialBalance;

        public decimal Debit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "扣款金额不能为负数。");
            }

            decimal appliedAmount = 0;
            lock (balanceLock)
            {
                if (balance >= amount)
                {
                    balance -= amount;
                    appliedAmount = amount;
                }
            }
            return appliedAmount;
        }

        public void Credit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "信用额度不能为负数。");
            }

            lock (balanceLock)
            {
                balance += amount;
            }
        }

        public decimal GetBalance()
        {
            lock (balanceLock)
            {
                return balance;
            }
        }
    }

}
