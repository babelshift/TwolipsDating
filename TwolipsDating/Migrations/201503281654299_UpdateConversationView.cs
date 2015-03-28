namespace TwolipsDating.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConversationView : DbMigration
    {
        public override void Up()
        {
            this.RemoveView("dbo.MessageConversations");
            this.CreateView("dbo.MessageConversations", @"
                select 
                    m.Id as 'MessageId', 
                    SenderApplicationUserId, 
					senderUser.UserName as 'SenderName',
					senderProfile.Id as 'SenderProfileId',
					senderProfileImage.FileName as 'SenderProfileImageFileName',
                    ReceiverApplicationUserId, 
					receiverUser.UserName as 'ReceiverName',
					receiverProfile.Id as 'ReceiverProfileId',
					receiverProfileImage.FileName as 'ReceiverProfileImageFileName',
                    DateSent, 
                    Body, 
                    MessageStatusId 
                from 
                    dbo.messages m
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
                    on m.senderapplicationuserid + '-' + m.receiverapplicationuserid = subquery.maxdateid 
                    and m.DateSent = subquery.maxdate
					inner join dbo.AspNetUsers senderUser on senderUser.Id = m.SenderApplicationUserId
					inner join dbo.AspNetUsers receiverUser on receiverUser.Id = m.ReceiverApplicationUserId
					inner join dbo.Profiles senderProfile on senderProfile.ApplicationUser_Id = senderUser.Id
					inner join dbo.Profiles receiverProfile on receiverProfile.ApplicationUser_Id = receiverUser.Id
					left join dbo.UserImages senderProfileImage on senderProfileImage.ApplicationUserId = senderUser.Id
					left join dbo.UserImages receiverProfileImage on receiverProfileImage.ApplicationUserId = receiverUser.Id");
        }
        
        public override void Down()
        {
            this.RemoveView("dbo.MessageConversations");
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
    }
}
