namespace MyProfile.Entity.ModelView.Section
{
    public class BaseSectionModelView : Select2ModelView    
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public string KeyWords { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public string Icon { get; set; }
        public int SectionTypeID { get; set; }
    }
}
