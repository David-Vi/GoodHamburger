namespace GoodHamburger.Core.Enums;

/// <summary>
/// Categorias de itens do cardápio — usadas nas regras de desconto.
/// </summary>
public enum MenuItemCategory
{
    Sandwich = 1,
    SideDish = 2,
    Drink = 3
}

/// <summary>
/// Identificadores fixos dos produtos. Preferimos enum + tabela no banco
/// a magic-strings; assim o compilador protege contra typos.
/// </summary>
public enum MenuItemId
{
    XBurger = 1,
    XEgg    = 2,
    XBacon  = 3,
    FrenchFries = 4,
    Soda        = 5
}
