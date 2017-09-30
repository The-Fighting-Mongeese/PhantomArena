using SQLite4Unity3d;

public class PlayerProfile  {

	[PrimaryKey, AutoIncrement, Unique]
	public int Id { get; set; }
    [Unique, Collation("NOCASE")]
	public string Name { get; set; }
	public int Level { get; set; }

	public override string ToString ()
	{
		return string.Format ("[Person: Id={0}, Name={1},  Level={2}]", Id, Name, Level);
	}

}