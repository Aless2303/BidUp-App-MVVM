//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BidUp_App
{
    using System;
    using System.Collections.Generic;
    
    public partial class Wallet
    {
        public int WalletID { get; set; }
        public int UserID { get; set; }
        public decimal Balance { get; set; }
        public System.DateTime LastUpdated { get; set; }
    
        public virtual User User { get; set; }
    }
}