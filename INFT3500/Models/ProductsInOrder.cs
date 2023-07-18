using System;
using System.Collections.Generic;

namespace INFT3500.Models;

public partial class ProductsInOrder
{
    public int? OrderId { get; set; }

    public int? ProduktId { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Stocktake? Produkt { get; set; }
}
