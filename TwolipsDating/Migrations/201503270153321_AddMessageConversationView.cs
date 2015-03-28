namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageConversationView : DbMigration
    {
        public override void Up()
        {
            this.CreateView("dbo.MessageConversations", @"
                select 
                    Id, 
                    SenderApplicationUserId, 
                    ReceiverApplicationUserId, 
                    DateSent, 
                    Body, 
                    MessageStatusId 
                from 
                    dbo.messages
                    join
                    (
	                    select 
	                        senderapplicationuserid + '-' + receiverapplicationuserid as 'maxdateid'
	                        , max(datesent) as 'maxdate'
	                    from 
                            dbo.Messages
	                    group by 
                            senderapplicationuserid + '-' + receiverapplicationuserid
                    ) subquery
                    on dbo.messages.senderapplicationuserid + '-' + dbo.messages.receiverapplicationuserid = subquery.maxdateid 
                    and dbo.messages.DateSent = subquery.maxdate
            ");
        }
        
        public override void Down()
        {
            this.RemoveView("dbo.MessageConversations");
        }
    }
}
