namespace TheBrain.Etls.Models
{
    public class Thought
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContentPath { get; set; } = string.Empty;
    }
}

/*
CREATE TABLE "Thoughts"(
"Id" varchar(36) PRIMARY KEY NOT NULL ,
"ACType" integer ,
"ActivationDateTime" bigint ,
"BackgroundColor" integer ,
"BrainId" varchar(36) ,
"CreationDateTime" bigint ,
"DisplayModificationDateTime" bigint ,
"ForegroundColor" integer ,
"ForgottenDateTime" bigint ,
"Kind" integer ,
"Label" varchar(140) ,
"LinksModificationDateTime" bigint ,
"ModificationDateTime" bigint ,
"Name" varchar(140) ,
"SyncUpdateId" varchar(36) ,
"SyncSentId" varchar(36) ,
"SyncUpdateDateTime" bigint ,
"ThoughtIconInfo" varchar(140) ,
"TypeId" varchar(36) )
  */