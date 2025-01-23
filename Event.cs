public class Event: IComparable {

    public DateOnly Date { get; set; }

    private string description;
    private string category;

    public string Description {
        get {
            return this.description;
        }

        set {
            ArgumentNullException.ThrowIfNullOrEmpty(value);

            // Check if description if too long
            if (value.Length > 250) {
                throw new ArgumentException("Description is too long!");
            }
            this.description = value;
        }
    }

    public string Category {
        get {
            return this.category;
        }

        set {
            if (string.IsNullOrEmpty(value)) {
                // If the provided value is null or empty, set the default category
                this.category = "...";
            }
            else {
                ArgumentNullException.ThrowIfNullOrEmpty(value);

                // Check if category is too long
                if (value.Length > 50) {
                    throw new ArgumentException("Category is too long!");
                }
                this.category = value;
            }
        }
    }

    // Constructor
    public Event(DateOnly date, string description, string category) {
        this.Date = new DateOnly(date.Year, date.Month, date.Day);
        this.Description = description;
        this.Category = category;
    }

    public override string ToString()
    {
        return $"{this.Date} {this.Description} ({this.Category})";
    }

    // IComparable implementation
    public int CompareTo(object obj) {
        if (obj == null) {
            return 1;
        }

        Event otherEvent = obj as Event;
        if (otherEvent != null) {
            return this.Date.CompareTo(otherEvent.Date);
        }
        else {
            throw new ArgumentException("Object is not an event");
        }
    }
}