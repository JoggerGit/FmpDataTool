﻿using FmpDataTool.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool
{
    /// <summary>
    /// DataContext
    /// </summary>
    public class DataContext : DbContext
    {
        private static DataContext _dataContext;
        private static readonly object lockObject = new object();

        internal DataContext(){}

        public static DataContext Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_dataContext == null)
                    {
                        _dataContext = new DataContext();
                    }
                    return _dataContext;
                }
            }
        }

        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.Instance["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Stock> Stocks { get; set; }
    }
}
