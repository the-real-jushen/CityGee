 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using Jtext103.EntityModel;

namespace Jtext103.Volunteer.VolunteerMessage
{
    public class Message:Entity
    {
        public Message(string messageFrom, Guid receiverId, string title, string text, List<string> pictures, string destinationLink, bool newBlank):base()
        {
            
            MessageFrom = messageFrom;
            ReceiverId = receiverId;
            Title = title;
            Text = text;
            Time = DateTime.Now;
            Pictures = pictures;
            DestinationLink = destinationLink;
            NewBlank = newBlank;
            HasRead = false;
            HasDeleted = false;
        }

        public Message(string messageFrom, Guid receiverId, string title, string text, List<string> pictures, string destinationLink, bool newBlank,string messageType):base()
        {
            MessageFrom = messageFrom;
            ReceiverId = receiverId;
            Title = title;
            Text = text;
            Time = DateTime.Now;
            Pictures = pictures;
            DestinationLink = destinationLink;
            NewBlank = newBlank;
            MessageType = messageType;
            HasRead = false;
            HasDeleted = false;
        }
        public Message()
        {
            Time = DateTime.Now;
            DestinationLink = "#";
        }

        /// <summary>
        /// the type of the message, like feed, msg。。。
        /// </summary>
        public string MessageType { get; set; }

        public bool IsBroadcast { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// （feed用的）图片
        /// </summary>
        public List<string> Pictures { get; set; }
        /// <summary>
        /// （feed用的）点击时跳转的链接
        /// </summary>
        public string DestinationLink { get; set; }
        /// <summary>
        /// （feed用的）是否在新标签中打开
        /// </summary>
        public bool NewBlank { get; set; }
        /// <summary>
        /// 发送的时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 接收用户的id
        /// </summary>
        public Guid ReceiverId { get; set; }
        /// <summary>
        /// 发送者的id（如果是系统等无id发送者，则为描述字符串）
        /// </summary>
        public string MessageFrom { get; set; }
        /// <summary>
        /// 标记接收用户是否读过
        /// </summary>
        public bool HasRead { get; set; }
        /// <summary>
        /// 标记用户是否删除
        /// </summary>
        public bool HasDeleted { get; set; }
    }
}
