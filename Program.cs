using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static string eventsFile = "events.csv"; // The file with the events

    static void Main(string[] args)
    {
        // If user didn't input any commands
        if (args.Length == 0)
        {
            Console.WriteLine("Give command: list, add, delete or help for more information.");
            return;
        }

        // Check the given command
        switch (args[0])
        {
            case "list":
                ListEvents(args);
                break;
            case "add":
                AddEvent(args);
                break;
            case "delete":
                DeleteEvents(args);
                break;
            case "help":
                EventsHelp(args);
                break;
            default:
                Console.WriteLine("Error: Unknown command");
                break;
        }
    }

    // Function to print instructions
    static void EventsHelp(string[] args) {
        Console.WriteLine("Usage:");

        Console.WriteLine("list");
        Console.WriteLine("\tList all events.");
        Console.WriteLine("\tOptional arguments:");
        Console.WriteLine("\t--date <date>: Filter events by date.");
        Console.WriteLine("\t--before-date <date>: Events before the given date.");
        Console.WriteLine("\t--after-date <date>: Events after the given date.");
        Console.WriteLine("\t--today: Filter events by today's date.");
        Console.WriteLine("\t--categories <category>,<category>: Filter events by one or multiple categories.");
        Console.WriteLine("\t--exclude <category>,<category>: Exclude the given categories from the results.");

        Console.WriteLine("add");
        Console.WriteLine("\tAdd a new event.");
        Console.WriteLine("\tRequired arguments:");
        Console.WriteLine("\t--date <date>: Date of the event (format: yyyy-MM-dd), date is today if left empty.");
        Console.WriteLine("\t--category <category>: Category of the event.");
        Console.WriteLine("\t--description <description>: Description of the event.");

        Console.WriteLine("delete");
        Console.WriteLine("\tDelete events.");
        Console.WriteLine("\tOptional arguments:");
        Console.WriteLine("\t--date <date>: Delete events with the specified date.");
        Console.WriteLine("\t--category <category>: Delete events with the specified category.");
        Console.WriteLine("\t--description <description>: Delete events with the specified description.");
        Console.WriteLine("\t--all: Delete all events.");
        Console.WriteLine("\t--dry-run: Preview what events are going to be deleted without deleting them.");

        Console.WriteLine("help");
        Console.WriteLine("\tDisplay this help message.");
    }

    // Function to list events
    static void ListEvents(string[] args)
    {
        // Read events from the file
        List<Event> events = ReadEvents();

        // Check if there are any events to display
        if (events.Count == 0)
        {
            Console.WriteLine("No events found.");
            return;
        }

        // Apply filters based on command-line arguments (if any)
        if (args.Length > 1)
        {
            events = FilterEvents(events, args);
        }

        // Display the filtered events
        foreach (var e in events)
        {
            Console.WriteLine($"{e.Date.ToShortDateString()} - {e.Description} - {e.Category}");
        }
    }

    // Function to filter the events
    static List<Event> FilterEvents(List<Event> events, string[] args)
    {
        List<Event> filteredEvents = events;

        // Iterate through command-line arguments to apply filters
        for (int i = 1; i < args.Length; i += 2)
        {
            if (args[i] == "--date" && i + 1 < args.Length)
            {
                if (DateOnly.TryParse(args[i + 1], out DateOnly filterDate))
                {
                    filteredEvents = filteredEvents.Where(e => e.Date == filterDate).ToList();
                }
            }
            else if (args[i] == "--categories" && i + 1 < args.Length)
            {
                string[] filterCategories = args[i + 1].Split(',');
                filteredEvents = filteredEvents.Where(e => filterCategories.Contains(e.Category)).ToList();
            }
            else if (args[i] == "--exclude" && i + 1 < args.Length)
            {
                string[] excludeCategories = args[i + 1].Split(',');
                filteredEvents = filteredEvents.Where(e => !excludeCategories.Contains(e.Category)).ToList();
            }
            else if (args[i] == "--today")
            {
                filteredEvents = filteredEvents.Where(e => e.Date == DateOnly.FromDateTime(DateTime.Today)).ToList();
            }
            else if (args[i] == "--before-date" && i + 1 < args.Length)
            {
                if (DateOnly.TryParse(args[i + 1], out DateOnly beforeDate))
                {
                    filteredEvents = filteredEvents.Where(e => e.Date < beforeDate).ToList();
                }
            }
            else if (args[i] == "--after-date" && i + 1 < args.Length)
            {
                if (DateOnly.TryParse(args[i + 1], out DateOnly afterDate))
                {
                    filteredEvents = filteredEvents.Where(e => e.Date > afterDate).ToList();
                }
            }
        }

        return filteredEvents;
    }

    // Function to add events
    static void AddEvent(string[] args) {

        // Parse command-line arguments to extract event information
        string date = null;
        string category = null;
        string description = null;
        bool dateGiven = false; // Bool to check if date was given

        for (int i = 1; i < args.Length; i += 2)
        {
            if (args[i] == "--date" && i + 1 < args.Length)
            {
                date = args[i + 1];
                dateGiven = true;
            }
            else if (args[i] == "--category" && i + 1 < args.Length)
            {
                category = args[i + 1];
            }
            else if (args[i] == "--description" && i + 1 < args.Length)
            {
                description = args[i + 1];
            }
        }

        // If user didn't give date then today is used
        if (!dateGiven) {
            date = DateTime.Today.ToString("yyyy-MM-dd");
        }

        // Check if required information is provided
        if (date == null || description == null)
        {
            Console.WriteLine("Error: Missing required information.");
            Console.WriteLine("Description is required, category is optional. If no date is provided, it's set to today.");
            Console.WriteLine("Usage: days add --date <date> --category <category> --description <description>");
            return;
        }

        if (!DateOnly.TryParse(date, out DateOnly eventDate)) {
            Console.WriteLine("Error: Invalid date format.");
            return;
        }

        // Create a new event
        Event newEvent = new Event(eventDate, description, category);

        // Save the event
        SaveEvent(newEvent);
    }

    // Function to write events to the csv file
    static void SaveEvent(Event e)
    {
        // Open the CSV file
        using (StreamWriter writer = new StreamWriter(eventsFile, true))
        {
            // Format the event data as a CSV line
            string csvLine = $"{e.Date},{e.Description},{e.Category}";
            // Write the CSV line to the file
            writer.WriteLine(csvLine);
        }
    }

    // Function to delete events
    static void DeleteEvents(string[] args)
    {
        // Read events from the file
        List<Event> events = ReadEvents();

        // If the file is empty or no events match the deletion criteria, return
        if (events.Count == 0)
        {
            Console.WriteLine("No events found or the file is empty.");
            return;
        }

        bool deleteAll = args.Contains("--all"); // Check if '--all' argument was given
        bool dryRun = args.Contains("--dry-run"); // Check if '--dry-run' argument was given

        // Check if any deletion criteria is provided
        bool hasCriteria = args.Any(arg => arg.StartsWith("--date") || arg.StartsWith("--description") || arg.StartsWith("--category"));

        // If neither '--all' nor any deletion criteria is provided, show error message
        if (!deleteAll && !hasCriteria)
        {
            Console.WriteLine("Usage: delete --all (to delete all events), delete --dry-run (to preview deletion), or specify deletion criteria like --date, --description, or --category");
            return;
        }

        // Delete all events
        if (deleteAll)
        {
            if (!dryRun)
            {
                // Delete all events
                int eventsDeleted = events.Count;
                events.Clear();
                SaveEvents(events);
                Console.WriteLine($"{eventsDeleted} events deleted.");
            }
            else // If '--dry-run' was given
            {
                foreach (var e in events)
                {
                    Console.WriteLine($"{e.Date.ToShortDateString()} - {e.Description} - {e.Category}");
                }
                Console.WriteLine($"{events.Count} events to be deleted.");
            }
        }
        else if (!dryRun) // If criteria was given but no dry run
        {
            // Delete events based on specified criteria
            int eventsDeleted = events.RemoveAll(e => ShouldDelete(e, args));
            if (eventsDeleted > 0)
            {
                SaveEvents(events);
                Console.WriteLine($"{eventsDeleted} events deleted.");
            }
            else
            {
                Console.WriteLine("No events match the deletion criteria.");
            }
        }
        else // Dry run
        {
            // Initialize a counter for events to be deleted
            int eventsToDeleteCount = 0;

            // Iterate through each event and check if it matches the deletion criteria
            foreach (var e in events)
            {
                bool shouldDelete = ShouldDelete(e, args);

                // If the event matches the deletion criteria, increment the counter and display the event
                if (shouldDelete)
                {
                    eventsToDeleteCount++;
                    Console.WriteLine($"{e.Date.ToShortDateString()} - {e.Description} - {e.Category}");
                }
            }

            Console.WriteLine($"{eventsToDeleteCount} events to be deleted.");

            // If no events match the deletion criteria, display a message
            if (eventsToDeleteCount == 0)
            {
                Console.WriteLine("No events match the deletion criteria.");
            }
        }

    }

    // Function to save events to the file
    static void SaveEvents(List<Event> events)
    {

        // Open the csv file
        using (StreamWriter writer = new StreamWriter(eventsFile))
        {
            // Write the header line
            writer.WriteLine("date,description,category");

            // Write every event to the correct format in the file
            foreach (var e in events)
            {
                string csvLine = $"{e.Date},{e.Description},{e.Category}";
                writer.WriteLine(csvLine);
            }
        }
    }

    // Function to check what events to delete based on criteria
    static bool ShouldDelete(Event e, string[] args)
    {
        // If arguments were given
        if (args.Length == 0)
        {
            return false;
        }

        // Iterate through all the arguments
        for (int i = 0; i < args.Length; i++)
        {
            // Check what events to delete based on given criteria
            if (args[i] == "--date")
            {
                // If the event date doesn't match the deleteDate, don't delete
                if (i + 1 < args.Length && DateOnly.TryParse(args[i + 1], out DateOnly deleteDate))
                {
                    if (e.Date != deleteDate)
                    {
                        return false;
                    }
                }
            }
            else if (args[i] == "--category")
            {
                // If the event category doesn't match, don't delete
                if (i + 1 < args.Length && e.Category != args[i + 1])
                {
                    return false;
                }
            }
            else if (args[i] == "--description")
            {
                // If the event description doesn't contain the given string, don't delete
                if (i + 1 < args.Length && !e.Description.Contains(args[i + 1]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Function to read events from the file
    static List<Event> ReadEvents() {
        List<Event> events = new List<Event>();

        // Check if the file exists
        if (File.Exists(eventsFile))
        {
            // Open the file
            using (StreamReader reader = new StreamReader(eventsFile))
            {
                // Skipping the first header row
                reader.ReadLine();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Split the line to fields
                    string[] fields = line.Split(',');

                    // Check that the line has correct amount of fields
                    if (fields.Length >= 3)
                    {
                        // Try parsing the date
                        if (DateOnly.TryParse(fields[0], out DateOnly date))
                        {
                            string description = fields[1];
                            string category = fields[2];

                            // Create a new event and add it to the list
                            Event newEvent = new Event(date, description, category);

                            events.Add(newEvent);
                        }
                        else
                        {
                            Console.WriteLine($"Invalid date format on line: {line}");
                        }
                    }
                }
            }
        }

        return events;
    }
}
