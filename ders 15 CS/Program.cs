using UIElements;
using System.Text;
using System.Text.Json;

struct Time
{
    public Time(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
    }

    public int Hour { get; set; }
    public int Minute { get; set; }

    public override string ToString()
        => $"{Hour:00}:{Minute:00}";
}

class User
{
    //Ad, Soyad, Email, Telefon
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public static User CreateUser()
    {
        User user = new User();
        user.ID = Guid.NewGuid();

        Console.Write("Enter Name: ");
        user.Name = Console.ReadLine() ?? "";

        Console.Write("Enter Surname: ");
        user.Surname = Console.ReadLine() ?? "";

        Console.Write("Enter Email: ");
        user.Email = Console.ReadLine() ?? "";

        Console.Write("Enter Phone: ");
        user.Phone = Console.ReadLine() ?? "";

        return user;
    }

    public override string ToString()
        => $"{Name} {Surname} ==> {ID}";
}

class Session
{
    public Time StartTime { get; set; }
    public Time EndTime { get; set; }
    public Guid OccupiedID { get; set; } = Guid.Empty;

    // public void SwitchCondition() => IsOccupied = !IsOccupied;
    public override string ToString() 
        => $"{StartTime.ToString()} ==> {EndTime.ToString()} | " 
            + (OccupiedID == Guid.Empty ? "Empty" : "Full");
}

class Doctor
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public byte Experience { get; set; }
    public List<Session> Sessions { get; set; } = new List<Session>();

    public Doctor()
    {
        Sessions.Add(new Session() { StartTime = new Time(9, 00), EndTime =  new Time(11, 00)});
        Sessions.Add(new Session() { StartTime = new Time(12, 00), EndTime = new Time(14, 00)});
        Sessions.Add(new Session() { StartTime = new Time(15, 00), EndTime = new Time(17, 00)});
    }

    public static Doctor CreateDoctor() 
        => new Doctor() { Name = Faker.NameFaker.FirstName(), Surname = Faker.NameFaker.LastName(), 
            Experience = (byte)Faker.NumberFaker.Number(1, 20) };

    public override string ToString() => $"{Name} {Surname} ==> {Experience} Years";
}

class Hospital
{
    public Dictionary<string, List<Doctor>> Departments{ get; set; } = new Dictionary<string,List<Doctor>>();
}

class Program
{
    static string[] DepartmentNames = {"Pediatriya", "Travmatologiya", "Stamologiya"};
    static List<User> Users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("users.json")) ?? new List<User>();
    static Hospital Hospital = JsonSerializer.Deserialize<Hospital>(File.ReadAllText("hospital.json")) ?? new Hospital();

    static void initHospital()
    {
        foreach (var name in DepartmentNames)
        {
            Hospital.Departments.Add(name, new List<Doctor>());
            for (int i = 0; i < 10; i++)
                Hospital.Departments[name].Add(Doctor.CreateDoctor());

        }
    }   

    static void Main(string[] args)
    {

        UI.ChangeEncoding(Encoding.Unicode, Encoding.Unicode);
        
        while (true)
        {
            User currentUser;

            if (Users.Count == 0) { currentUser = User.CreateUser(); Users.Add(currentUser); }
            else
            {
                int userChoice = UI.GetChoice("Do you want to:", new string[] { "Log In as Existing User", "Sign UP as a New User" }, true);
                if (userChoice == UI.ESCAPE) break;
                else if (userChoice == 0)
                {
                    int logInChoice = UI.GetChoice("Choose One:", Users.ToArray(), true);
                    if (logInChoice == UI.ESCAPE) continue;
                    else currentUser = Users[logInChoice];
                }
                else { currentUser = User.CreateUser(); Users.Add(currentUser); }
            }


            List<Doctor> doctors = Hospital.Departments[DepartmentNames[UI.GetChoice("Choose Department:", DepartmentNames)]];
            Doctor doctor = doctors[UI.GetChoice("Choose Doctor:", doctors.ToArray())];
            Session session = doctor.Sessions[UI.GetChoice("Choose Session", doctor.Sessions.ToArray())];

            if(session.OccupiedID == currentUser.ID)
            {
                 Console.WriteLine($"Dear {currentUser.Name} {currentUser.Surname}, " +
                    $"Your session between {session.StartTime} {session.EndTime} has been CANCELLED.");
                session.OccupiedID = Guid.Empty;
            }
            else if(session.OccupiedID == Guid.Empty)
            {
                Console.WriteLine($"Dear {currentUser.Name} {currentUser.Surname}, " +
                    $"Your session between {session.StartTime} {session.EndTime} has been OCCUPIED.");
                session.OccupiedID = currentUser.ID;
            }
            else Console.WriteLine("You can't CANCEL a SESSION not Made by YOU.");

            Console.Write("Enter any key to Continue...");
            Console.ReadKey();
        }

        File.WriteAllText("users.json", JsonSerializer.Serialize(Users));
        File.WriteAllText("hospital.json", JsonSerializer.Serialize(Hospital));
    }
}