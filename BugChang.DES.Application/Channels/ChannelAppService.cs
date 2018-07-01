using System;
using System.Collections.Generic;
using BugChang.DES.Application.Channels.Dtos;
using BugChang.DES.Core.Exchanges.Channel;

namespace BugChang.DES.Application.Channels
{
    public class ChannelAppService : IChannelAppService
    {
        public List<ChannelListDto> GetChannels()
        {
            var channelList = new List<ChannelListDto>();
            foreach (var item in Enum.GetValues(typeof(EnumChannel)))
            {
                var channel = new ChannelListDto
                {
                    Id = (int)item,
                    Name = item.ToString()
                };
                channelList.Add(channel);
            }

            return channelList;
        }
    }
}
