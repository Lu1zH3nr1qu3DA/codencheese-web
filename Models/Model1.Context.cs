﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebHamburgueria.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbhamburgueriaEntities : DbContext
    {
        public dbhamburgueriaEntities()
            : base("name=dbhamburgueriaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Administrador> Administrador { get; set; }
        public virtual DbSet<ItensPedido> ItensPedido { get; set; }
        public virtual DbSet<Pedido> Pedido { get; set; }
        public virtual DbSet<Produto> Produto { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
    }
}
