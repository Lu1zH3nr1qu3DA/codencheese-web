//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ItensPedido
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public string NomeProduto { get; set; }
        public decimal PrecoProduto { get; set; }
        public Nullable<int> IdProduto { get; set; }
    
        public virtual Pedido Pedido { get; set; }
    }
}
