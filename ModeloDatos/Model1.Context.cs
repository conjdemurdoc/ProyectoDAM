﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModeloDatos
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VeterinariaEntities : DbContext
    {
        public VeterinariaEntities()
            : base("name=VeterinariaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TBLCITAPREVIA> TBLCITAPREVIA { get; set; }
        public virtual DbSet<TBLCLIENTES> TBLCLIENTES { get; set; }
        public virtual DbSet<TBLMASCOTAS> TBLMASCOTAS { get; set; }
        public virtual DbSet<TBLPRODUCTOS> TBLPRODUCTOS { get; set; }
        public virtual DbSet<TBLPROVEEDORES> TBLPROVEEDORES { get; set; }
        public virtual DbSet<TBLTICKETS> TBLTICKETS { get; set; }
        public virtual DbSet<TBLUSUARIOS> TBLUSUARIOS { get; set; }
    }
}
