namespace Models;

public class Specialities
{
    public int Id { get; set; }
    public string SpecialityName { get; set; }

}

public class SpecialitiesLink
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int SpecialityId { get; set; }

}