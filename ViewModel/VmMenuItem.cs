using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagement.ViewModel
{
    public class VmMenuItem
    {
        public long Id { get; set; }
        public bool IsParent { get; set; }
        public long? ParentId { get; set; }
        public string ModuleName { get; set; }
        public string MenuName { get; set; }
        public string MenuURL { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ActiveLinkName { get; set; }
        public string IconAttribute { get; set; }
        public string MenuStyle { get; set; }
        public int MenuOrder { get; set; }
        public bool MenuVisibility { get; set; }
        public virtual List<VmMenuItem> ChildMenus { get; set; }
        public string ParentMenuName { get; set; }
        public SelectList ParentMenus { get; set; }
        public SelectList Modules { get; set; }
    }
}