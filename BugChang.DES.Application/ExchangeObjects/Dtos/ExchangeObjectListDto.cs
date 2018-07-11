﻿using BugChang.DES.Application.Commons;

namespace BugChang.DES.Application.ExchangeObjects.Dtos
{
    public class ExchangeObjectListDto : BaseDto
    {
        public string Name { get; set; }

        public string ObjectType { get; set; }

        public string Value { get; set; }

        public string ValueText { get; set; }
    }
}
