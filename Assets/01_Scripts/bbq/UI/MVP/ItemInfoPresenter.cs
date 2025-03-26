public class ItemInfoPresenter
{
    private readonly IItemInfoView _view;
    private readonly InventoryManager _inventoryManager;

    public ItemInfoPresenter(IItemInfoView view, InventoryManager inventoryManager)
    {
        _view = view;
        _inventoryManager = inventoryManager;
    }

    public void OnItemHovered(Item item)
    {
        if (item != null)
        {
            _view.UpdateItemInfo(item.nameStr, item.GetDescription().ToString());
        }
    }
}