using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Business
{
    public class ProfileService : BaseService
    {
        public int SendMessage(string senderUserId, string receiverUserId, string subject, string body)
        {
            // don't allow us to send messages to our self
            //if(senderUserId == receiverUserId)
            //{
            //    return 0;
            //}

            Message message = db.Messages.Create();

            ApplicationUser senderUser = new ApplicationUser() { Id = senderUserId };
            db.Users.Attach(senderUser);
            message.SenderApplicationUser = senderUser;

            // if 
            if (senderUserId != receiverUserId)
            {
                ApplicationUser receiverUser = new ApplicationUser() { Id = receiverUserId };
                db.Users.Attach(receiverUser);
                message.ReceiverApplicationUser = receiverUser;
            }
            else
            {
                message.ReceiverApplicationUser = senderUser;
            }

            message.Body = body;
            message.Subject = subject;
            message.DateSent = DateTime.Now;
            message.MessageStatusId = (int)MessageStatusValue.Unread;

            db.Messages.Add(message);
            return db.SaveChanges();
        }

        public Profile GetProfile(string userId)
        {
            var profile = from user in db.Users
                          where user.Id == userId
                          select user.Profile;

            return profile.FirstOrDefault();
        }

        public IReadOnlyCollection<Gender> GetGenders()
        {
            var genders = from gender in db.Genders
                          select gender;

            return genders.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Country> GetCountries()
        {
            var countries = from country in db.Countries
                            select country;

            return countries.ToList().AsReadOnly();
        }

        public City GetCityByName(string cityName)
        {
            var city = (from cities in db.Cities
                        where cities.Name == cityName
                        select cities).FirstOrDefault();
            return city;
        }

        public City GetCityByZipCode(string zipCode)
        {
            var city = (from zipCodes in db.ZipCodes
                        where zipCodes.ZipCodeId == zipCode
                        select zipCodes.City).FirstOrDefault();
            return city;
        }

        public void CreateProfile(int genderId, int? zipCode, int cityId, string userId, DateTime birthday)
        {
            ApplicationUser user = new ApplicationUser() { Id = userId };

            db.Users.Attach(user);

            Profile p = db.Profiles.Create();
            p.Birthday = birthday;
            p.ApplicationUser = user;
            p.CityId = cityId;
            p.GenderId = genderId;
            p.ZipCode = zipCode;

            db.Profiles.Add(p);
            db.SaveChanges();
        }

        public int GetUnreadMessageCount(string userId)
        {
            int unreadMessageCount = (from messages in db.Messages
                                      where messages.ReceiverApplicationUserId == userId
                                      where messages.MessageStatusId == (int)MessageStatusValue.Unread
                                      select messages).Count();
            return unreadMessageCount;
        }
    }
}