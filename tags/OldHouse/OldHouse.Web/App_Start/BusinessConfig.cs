using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jtext103.Repository;
using Jtext103.Repository.Interface;
using Jtext103.MongoDBProvider;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using AutoMapper;
using Jtext103.BlogSystem;
using Jtext103.EntityModel;
using Jtext103.Identity.Service;
using OldHouse.Web.Models;

namespace OldHouse.Web.App_Start
{
    /// <summary>
    /// config all biusiness here, like db and data mapping
    /// </summary>
    public class BusinessConfig
    {
        public static HouseService MyHouseService { get; set; }
        public static EntityService<FeedBackEntity> MyFeedbackService { get; set; }
        /// <summary>
        /// config the business logic
        /// </summary>
        public static void ConfigBusiness()
        {
            //todo ioc is really needed here
            MongoDBRepository<House> houseBb = new MongoDBRepository<House>(@"mongodb://127.0.0.1:27017", "OldHouseDb", "House");
            MongoDBRepository<BlogPostEntity> CheckInDb = new MongoDBRepository<BlogPostEntity>(@"mongodb://127.0.0.1:27017", "OldHouseDb", "CheckIn");
            MongoDBRepository<OldHouseUser> UserDb = new MongoDBRepository<OldHouseUser>(@"mongodb://127.0.0.1:27017", "OldHouseDb", "OldHouseUser");
            MongoDBRepository<OldHouseUserProfile> ProfileDb = new MongoDBRepository<OldHouseUserProfile>(@"mongodb://127.0.0.1:27017", "OldHouseDb", "OldHouseUserProfile");

            MongoDBRepository<FeedBackEntity> FeedbackDb = new MongoDBRepository<FeedBackEntity>(@"mongodb://127.0.0.1:27017", "OldHouseDb", "Feedback");

            MyFeedbackService=new EntityService<FeedBackEntity>(FeedbackDb);
            
            MyHouseService = new HouseService(
                houseBb, 
                new BlogPostService(CheckInDb,null), 
                new EntityService<OldHouseUserProfile>( ProfileDb), 
                new UserManager<OldHouseUser>(UserDb));
            ConfigDtoMapping();
        }

        public static void ConfigDtoMapping()
        {
            Mapper.CreateMap<House, HouseBrief>();
            Mapper.CreateMap<House, HouseDetail>();
            
            //all the checkin is regarded to be gcj-02 coords, but the current location may be using WGS84 coords,if there are notable drift,
            //we will transform the cuurent location coords to GJC-02
            //and we now assume the coords of the houses are GCJ-02, if not we will transfor there house coords to GCJ-02 coords
            //map New CheckinDTO to a real checkin object
            //child map
            Mapper.CreateMap<Asset,string>().ConvertUsing(des=>des.Path);
            Mapper.CreateMap<string, Asset>()
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Asset.IMAGE));  //the defual mapping is to image you can chage this later 

            Mapper.CreateMap<CheckInDto, CheckIn>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>HouseService.GetGeoPoint(src.Lnt+@";"+src.Lat)))
                .ForMember(dest=>dest.User,opt=>opt.MapFrom(src=> new BasicUser {Id=src.UserId}))
                .ForMember(dest => dest.Asset, opt => opt.MapFrom(src => src.Images));
            //map checkin into checkin dto
            Mapper.CreateMap<CheckIn, CheckInDto>()
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.Location.coordinates[1]))
                .ForMember(dest => dest.Lnt, opt => opt.MapFrom(src => src.Location.coordinates[0]))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>src.Asset));


            Mapper.CreateMap<OldHouseUser, UserDisplayDto>();

            //register user model to real user
            Mapper.CreateMap<RegisterViewModel, OldHouseUser>()
                .ForMember(dest=>dest.PasswordHash,opt=>opt.MapFrom(src=>src.Password));

        }
    }
        
}