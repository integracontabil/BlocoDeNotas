using Supabase;
using Postgrest.Attributes;

[Table("contacts")]
public class Contact : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("phone")]
    public string Phone { get; set; }

    [Column("inserted_at")]
    public DateTimeOffset InsertedAt { get; set; }
}
