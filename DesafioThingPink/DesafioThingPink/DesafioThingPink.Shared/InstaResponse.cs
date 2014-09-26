﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DesafioThingPink
{
    public class Meta
    {
        public int code { get; set; }
    }

    public class InstaLocation
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public int? id { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
        public List<object> data { get; set; }
    }

    public class Datum2
    {
        public string username { get; set; }
        public string profile_picture { get; set; }
        public string id { get; set; }
        public string full_name { get; set; }
    }

    public class Likes
    {
        public int count { get; set; }
        public List<Datum2> data { get; set; }
    }

    public class LowResolution
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnail
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class StandardResolution
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Images
    {
        public LowResolution low_resolution { get; set; }
        public Thumbnail thumbnail { get; set; }
        public StandardResolution standard_resolution { get; set; }
    }

    public class From
    {
        public string username { get; set; }
        public string profile_picture { get; set; }
        public string id { get; set; }
        public string full_name { get; set; }
    }

    public class Caption
    {
        public string created_time { get; set; }
        public string text { get; set; }
        public From from { get; set; }
        public string id { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public string website { get; set; }
        public string profile_picture { get; set; }
        public string full_name { get; set; }
        public string bio { get; set; }
        public string id { get; set; }
    }

    public class Datum
    {
        public object attribution { get; set; }
        public List<object> tags { get; set; }
        public InstaLocation location { get; set; }
        public Comments comments { get; set; }
        public string filter { get; set; }
        public string created_time { get; set; }
        public string link { get; set; }
        public Likes likes { get; set; }
        public Images images { get; set; }
        public List<object> users_in_photo { get; set; }
        public Caption caption { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public User user { get; set; }
    }

    public class InstaRootObject
    {
        public Meta meta { get; set; }
        public List<Datum> data { get; set; }
    }
}
