using System.Collections.Generic;

namespace Aika_Packet_Sniffer.Model
{
    public class TypesListView
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string TypeU { get; set; }
        public string ValueU { get; set; }
    }

    public class FormatTypesListView
    {
        public List<TypesListView> TypesListViews { get; }

        public FormatTypesListView()
        {
            TypesListViews = new List<TypesListView>();
            
            TypesListViews.Add(new TypesListView
            {
                Type = 
            });
        }
    }
}