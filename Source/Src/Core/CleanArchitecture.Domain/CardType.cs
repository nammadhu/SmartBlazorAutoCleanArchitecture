namespace CleanArchitecture.Domain
    {
    //town table
    //townitemtype master data table
    //townid+ townitem table

    //fetch city related all entities at once & store on client side
    //display city title.subtitle,decription
    //priority & posible solution
    //events
    //business
    //doctor
    //school
    //college
    //real estate
    //buyorsale
    //openissue
    //report complaint
    //jobs vacancies & registrqation of job looking candidates in town
    //feedback or contact
    //no db entity

    //db entity
    public class CardType : AuditableBaseEntity, IMasterData, IAuditableBaseEntity<int>
        {//this is only masterdata
        [Key]
        public override int Id { get; set; }

        public CardType()
            {
            Name = string.Empty;
            ShortName = string.Empty;
            }

        public CardType(int id, string name, string iconClass, string iconColor)
            {
            Id = id;
            Name = name;
            ShortName = name;
            IconClass = iconClass;
            IconColor = iconColor;
            }

        //public CardType(int id, string name, string iconPath, string iconColor, string shortName) : this(id, name, iconPath, iconColor)
        //{
        //    ShortName = shortName;
        //}
        public CardType(int id, string name, string iconClass, string iconColor, string html5MarkupIcon)
            : this(id, name, iconClass, iconColor)
            {
            ShortName = name;
            IconMarkupString = html5MarkupIcon;
            }

        /*
        public CardType(string name)
            {
            Name = name;
            ShortName = name;
            }
        public CardType(string name, string shortName)
            {
            Name = name;
            ShortName = shortName;
            }
          public CardType(int id, string name)
        {
            Id = id;
            Name = name;
            ShortName = name;
        }
        */

        //[Key]
        //public int Id { get; set; }
        //this should not  be appeared to front end screen to users
        //    public byte ApplicationTypeId { get; set; } = 1;//1 for townitem,2 for holige products
        public string Name { get; set; }//1:Town,2doctor,business,event,advertisement

        public string ShortName { get; set; }
        public string IconClass { get; set; } = "mdi-cards-outline";

        public string IconMarkupString { get; set; } = "&#128295;";
        public string IconColor { get; set; } = "#4CAF50";

        public string? Description { get; set; }
        public int Price { get; set; } = 1;//100
        public byte? PriorityOrder { get; set; } = 1;

        public byte RequiredApprovalCount { get; set; } = 1;

        //todo later will make it up in each card by user charging onrequest moving up or first kind of

        //not using this,instead using automapper
        //public void Update(string name, string shortName, string description, int price)
        //    {
        //    Name = name;
        //    ShortName = shortName;
        //    Description = description;
        //    Price = price;
        //    }
        /*
        public static CardType? Get(int id)
            {
            return StandardList.Find(x => x.Id == id);
            }
        public static CardType? GetFirst(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        public static List<CardType>? GetList(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        */
        }
    }
