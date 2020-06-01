namespace MapR.Data.Models {
    public interface MapModel { 

        string Id { get; set; }
        string GameId { get; set; }
		string ImageUri { get; set; }
        string Name { get; set; }
        bool IsActive { get; set; }
        bool IsPrimary { get; set; }

    }
}