/*
Author: Angelo Romel Lopez, BSc Computing Science 3rd Yr
Module: Web Services
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    /// <summary>
    /// Private class attributes
    /// </summary>
    private static bool validUser = false;//true if a user is logged in, false otherwise.
    private XmlDocument xml_Data;//Stores the Xml document.
    private String file_Name;//Stores a valid file/path name for the xml document.

    /// <summary>
    /// Default empty constructor.
    /// </summary>
    public Service () {
    }

    
    /// <summary>
    /// This method will return a valid name and path of the youtubeplaylist xml.
    /// </summary>
    /// <returns>A valid path and filename.</returns>
    private String fileName()
    {
        return Server.MapPath("App_Data\\youtubeplaylist.xml");
    }

    /// <summary>
    /// Loads the XML data file from a valid filename (path: App_Data\...) into an xml document
    /// if the xml document is empty, and return a valid xml document.Otherwise, just return an existing
    /// xml document.
    /// </summary>
    /// <returns>A valid xml document.</returns>
    private XmlDocument xmlData()
    {
        if(xml_Data == null)
        {
            getDataFile();
        }
        return xml_Data;
    }

    /// <summary>
    /// Checks if the xml file exists in the App_Data
    /// and loads the file in an xml document type variable.
    /// </summary>
    /// <returns>true if file exists, false otherwise.</returns>
    private bool getDataFile()
    {
        if (System.IO.File.Exists(fileName()))
        {
            xml_Data = new XmlDocument();
            xml_Data.Load(fileName());
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the xml document into the xml file.
    /// </summary>
    public void saveDataFile()
    {
        if(xml_Data != null)
        {
            xml_Data.Save(fileName());
        }
    }

    /// <summary>
    /// This method will return a valid name and path of the authenticate xml file that 
    /// contains the list of valid users and passwords used for authentication.
    /// </summary>
    /// <returns></returns>
    private XmlDocument xmlUser()
    {
        XmlDocument xml_User = new XmlDocument();
        xml_User.Load(Server.MapPath("App_Data\\authenticate.xml"));
        return xml_User;
    }

    /// <summary>
    /// Returns an xml element containing the username and password of a selected user.
    /// </summary>
    /// <param name="username">The user's username</param>
    /// <returns></returns>
    private XmlElement getUserCredentials(string username)
    {
        XmlElement validUser;
        string xPath = "//user[@name='" + username + "']";
        validUser = (XmlElement) xmlUser().DocumentElement.SelectSingleNode(xPath);
        return validUser;
    }
    
    /// <summary>
    /// Retrieves the playlist collection of a client.
    /// </summary>
    /// <param name="nickname">The nickname will be compared with the nickname attribute
    /// of the client node.</param>
    /// <returns>Xml element containing a client's playlist collection.</returns>
    private XmlElement getClientList(string nickname)
    {
        XmlElement xmlItems;
        string xPath = "//client[@nickname='" + nickname + "']";
        xmlItems = (XmlElement) xmlData().DocumentElement.SelectSingleNode(xPath);
        return xmlItems;
    }

    /// <summary>
    /// Retrieves a playlist's information and collection tracks.
    /// </summary>
    /// <param name="playname">The name attribute of the playlist element.</param>
    /// <returns>Xml element that contains a playlist's additional information and
    /// collection of tracks.</returns>
    private XmlElement getPlayList(string playname)
    {
        XmlElement xmlItems;
        string xPath = "//playlist[@playname='" + playname + "']";
        xmlItems = (XmlElement) xmlData().DocumentElement.SelectSingleNode(xPath);
        return xmlItems;
    }

    /// <summary>
    /// Retrieves a track's information such as title, location, duration and
    /// rank.
    /// </summary>
    /// <param name="id">The attribute of a track that contains a 
    /// auto-generated GUID value.</param>
    /// <returns>A collection of track information.</returns>
    private XmlElement getTrackList(string id)
    {
        XmlElement xmlItems;
        string xPath = "//track[@id='" + id + "']";
        xmlItems = (XmlElement)xmlData().DocumentElement.SelectSingleNode(xPath);
        return xmlItems;
    }

    /// <summary>
    /// Creates a new client with a nickname attribute
    /// </summary>
    /// <param name="nickname"></param>
    /// <returns>True if a new client is created, false if the client already
    /// exists.</returns>
    private bool insertNewClient(string nickname)
    {
        if(getClientList(nickname) != null)
        {
            return false;
        }
        else
        {
            XmlElement client = xmlData().CreateElement("client");
            XmlAttribute clientAtt = xmlData().CreateAttribute("nickname");
            clientAtt.Value = nickname;
            client.Attributes.Append(clientAtt);
            xmlData().DocumentElement.AppendChild(client);
            return true;
        }
    }

    /// <summary>
    /// Creates a new element identified by the name parameter and gives
    /// it a value.
    /// </summary>
    /// <param name="name">The name to identify the element.</param>
    /// <param name="value">The value of the element.</param>
    /// <returns>The newly created element.</returns>
    private XmlElement newElement(string name, string value)
    {
        XmlElement element = xmlData().CreateElement(name);
        element.InnerText = value;
        return element;
    }

    /// <summary>
    /// Tests the value of the score parameter to satisfy the following conditions:
    /// a. A valid number.
    /// b. Value is in the range of 0-5.
    /// </summary>
    /// <param name="score">The score will be parsed to an integer before being tested.</param>
    /// <returns>True if the score is valid, false otherwise.</returns>
    private bool isValidScore(string score)
    {
        try
        {
            if( Int32.Parse(score) >= 0 && Int32.Parse(score) <= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Genereates a GUID - globally unique identifier to be used as the ID of a track.
    /// </summary>
    /// <returns>GUID value.</returns>
    private string generateGuid()
    {
        return System.Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Creates a new playlist for a particular client.
    /// </summary>
    /// <param name="nickname">The client's nickname</param>
    /// <param name="playlistname">The name of the playlist to be created.</param>
    /// <returns>True if the playlist was successfully created.</returns>
    private bool insertNewPlaylist(string nickname, string playlistname)
    {
        try
        {
            XmlElement new_Element;
            XmlElement clientList = getClientList(nickname);
            XmlElement newPlaylist = xmlData().CreateElement("playlist");
            XmlAttribute playlistAtt = xmlData().CreateAttribute("playname");
            playlistAtt.Value = playlistname;
            newPlaylist.Attributes.Append(playlistAtt);
            new_Element = newElement("score", "0");
            newPlaylist.AppendChild(new_Element);
            new_Element = newElement("votecount", "0");
            newPlaylist.AppendChild(new_Element);
            clientList.AppendChild(newPlaylist);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new track for a particualr playlist that contains a link to a youtube
    /// video.
    /// </summary>
    /// <param name="playname">The name of the playlist where the track is to be created.</param>
    /// <param name="trackTitle">The title of the track to be created.</param>
    /// <param name="urlLocation">The shorl url of the youtube video.</param>
    /// <param name="duration">The duration of the video</param>
    /// <returns>True if the track was successfully created, false otherwise.</returns>
    private bool insertNewTrack(string playname, string trackTitle, string urlLocation,
        string duration)
    {
        try
        {
            XmlElement new_Element;
            XmlElement playlist = getPlayList(playname);
            XmlElement newTrack = xmlData().CreateElement("track");
            XmlAttribute trackAtt = xmlData().CreateAttribute("id");
            trackAtt.Value = generateGuid();
            newTrack.Attributes.Append(trackAtt);
            new_Element = newElement("title", trackTitle);
            newTrack.AppendChild(new_Element);
            new_Element = newElement("location", urlLocation);
            newTrack.AppendChild(new_Element);
            new_Element = newElement("duration", duration);
            newTrack.AppendChild(new_Element);
            new_Element = newElement("rank", "");
            newTrack.AppendChild(new_Element);
            playlist.AppendChild(newTrack);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /*
    Start of web methods ******************************************************************
    */

    /// <summary>
    /// Log in to the web service to authenticate user and allow
    /// access to the web methods.
    /// </summary>
    /// <param name="username">The user's username</param>
    /// <param name="password">The user's password</param>
    /// <returns>True if the username and password exists, false otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool login(string username, string password)
    {   
        try {
            XmlElement user = getUserCredentials(username);
            XmlNode userNode = user.SelectSingleNode("//user[@name='" + username + "']");
            if (userNode["password"].InnerText.Trim() == password)
            {
                validUser = true;
                return true;
            }
            else
            {
                validUser = false;
                return false;
            }
        }
        catch
        {
            return false;
        }        
    }

    /// <summary>
    /// Log out of the web service and prevent access to
    /// the web methods.
    /// </summary>
    [System.Web.Services.WebMethod]
    public void logout()
    {
        validUser = false;
    }

    /// <summary>
    /// Retrieves a client's playlist collection.
    /// </summary>
    /// <param name="nickname">The client's nickname.</param>
    /// <returns>The nodes in the collection.</returns>
    [System.Web.Services.WebMethod]
    public string getClientPlaylistCollection(string nickname)
    {
        if (validUser)
        {
            return getClientList(nickname).OuterXml;
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Retrieves a client's particular playlist identified by it's
    /// playname attribute.
    /// </summary>
    /// <param name="nickname">The client's nickname.</param>
    /// <param name="playname">The playlist's playname.</param>
    /// <returns>The playlist information and the tracks contained within.</returns>
    [System.Web.Services.WebMethod]
    public string getPlaylist(string nickname, string playname)
    {
        if (validUser)
        {
            XmlElement playlists = getClientList(nickname);
            XmlElement playlist = (System.Xml.XmlElement)playlists.SelectSingleNode("//playlist[@playname='" + playname + "']");
            if (playlist != null)
            {
                return playlist.OuterXml;
            }
            else
            {
                return "</playlist>";
            }
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="nickname">The client's nickname attribute.</param>
    /// <returns>True if client was created successfully, false otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool createNewClient(string nickname)
    {
        if (validUser)
        {
            if (insertNewClient(nickname))
            {
                saveDataFile();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new playlist for a particular client.
    /// </summary>
    /// <param name="nickname">The client's nickname.</param>
    /// <param name="playlistname">The name of the playlist to be created.</param>
    /// <returns>True if the playlist was successfully created.</returns>
    [System.Web.Services.WebMethod]
    public bool createNewPlayList(string nickname, string playlistname)
    {
        if (validUser)
        {
            if (insertNewPlaylist(nickname, playlistname))
            {
                saveDataFile();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Create a new track for a particular playlist the contains a link to a
    /// youtube video.
    /// </summary>
    /// <param name="playname">The name of the playlist.</param>
    /// <param name="trackTitle">The title of the track.</param>
    /// <param name="urlLocation">A short url location of the youtube video.</param>
    /// <param name="duration">Duration of the track</param>
    /// <returns>True of the track was successfully created.</returns>
    [System.Web.Services.WebMethod]
    public bool createNewTrack(string playname, string trackTitle, string urlLocation,
        string duration)
    {
        if (validUser)
        {
            if (insertNewTrack(playname, trackTitle, urlLocation, duration))
            {
                saveDataFile();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Update/change a client's nickname.
    /// </summary>
    /// <param name="nickname">The client's current nickname.</param>
    /// <param name="newClientNickName">The client's new nickname.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool updateClientName(string nickname, string newClientNickName)
    {
        if (validUser)
        {
            try
            {
                XmlElement clients = getClientList(nickname);
                XmlElement client = (System.Xml.XmlElement)clients.SelectSingleNode("//client[@nickname='" + nickname + "']");
                client.Attributes[0].Value = newClientNickName;
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Update/change a playlist's name.
    /// </summary>
    /// <param name="nickname">The client's nickname where the playlist is located.</param>
    /// <param name="playname">The playlist's current name.</param>
    /// <param name="newPlayname">The playlist's new name.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool updatePlaylistName(string nickname, string playname, string newPlayname)
    {
        if (validUser)
        {
            try
            {
                XmlElement playlists = getClientList(nickname);
                XmlElement playlist = (System.Xml.XmlElement)playlists.SelectSingleNode("//playlist[@playname='" + playname + "']");
                playlist.Attributes[0].Value = newPlayname;
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieve track information for a particular playlist.
    /// </summary>
    /// <param name="playname">The name of the playlist.</param>
    /// <param name="trackID">The ID of the track to retrieve.</param>
    /// <returns>Track information like title, location, duration and rank.</returns>
    [System.Web.Services.WebMethod]
    public string getTrackInfo(string playname, string trackID)
    {
        if (validUser)
        {
            try
            {
                XmlElement playlist = getPlayList(playname);
                XmlElement trackList = (System.Xml.XmlElement)playlist.SelectSingleNode("//track[@id='" + trackID + "']");
                return trackList.OuterXml;
            }
            catch
            {
                return "</track>";
            }
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Update/change a track information.
    /// </summary>
    /// <param name="playname">The name of the playlist where the track is located.</param>
    /// <param name="trackID">The ID of the track to be updated.</param>
    /// <param name="title">The new title of the track.</param>
    /// <param name="location">The new location of the track.</param>
    /// <param name="duration">The new duration of the track.</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public bool updateTrack(string playname, string trackID, string title, string location, string duration)
    {
        if (validUser)
        {
            try
            {
                XmlElement playlist = getPlayList(playname);
                XmlElement trackList = (System.Xml.XmlElement)playlist.SelectSingleNode("//track[@id='" + trackID + "']");
                trackList["title"].InnerText = title;
                trackList["location"].InnerText = location;
                trackList["duration"].InnerText = duration;
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a particular track from a playlist.
    /// </summary>
    /// <param name="playname">The name of the playlist where the track to be removed
    /// is located.</param>
    /// <param name="trackID">The ID of the track to be removed.</param>
    /// <returns>True if the track was removed successfully. False otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool removeTrack(string playname, string trackID)
    {
        if (validUser)
        {
            try
            {
                XmlElement playlist = getPlayList(playname);
                XmlNode trackList = (System.Xml.XmlElement)playlist.SelectSingleNode("//track[@id='" + trackID + "']");
                trackList.ParentNode.RemoveChild(trackList);
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a particular playlist.
    /// </summary>
    /// <param name="playname">The playlist identified by it's name.</param>
    /// <returns>True if the playlist was removed successfully. False otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool removePlaylist(string playname)
    {
        if (validUser)
        {
            try
            {
                XmlElement playlist = getPlayList(playname);
                XmlNode playlistNode = playlist.SelectSingleNode("//playlist[@playname='" + playname + "']");
                playlistNode.ParentNode.RemoveChild(playlistNode);
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a particular client from the xml file.
    /// </summary>
    /// <param name="nickname">The client identified by it's nickname.</param>
    /// <returns>True if the client was removed successfully. False otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool removeClient(string nickname)
    {
        if (validUser)
        {
            try
            {
                XmlElement client = getClientList(nickname);
                XmlNode clientNode = client.SelectSingleNode("//client[@nickname='" + nickname + "']");
                clientNode.ParentNode.RemoveChild(clientNode);
                saveDataFile();
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Place a vote on a particular playlist. A user can vote from 0 to 5.
    /// </summary>
    /// <param name="playname">The name of the playlist to vote on.</param>
    /// <param name="score">The score from 0 to 5.</param>
    /// <returns>True if the vote is valid or was succesful. False otherwise.</returns>
    [System.Web.Services.WebMethod]
    public bool voteOnPlaylist(string playname, double score)
    {
        if (validUser)
        {
            try
            {
                if (isValidScore(score.ToString()))
                {
                    XmlElement playlist = getPlayList(playname);
                    XmlNode playlistNode = playlist.SelectSingleNode("//playlist[@playname='" + playname + "']");

                    playlistNode["votecount"].InnerText = (Convert.ToInt32(playlistNode["votecount"].InnerText.Trim()) + 1).ToString();

                    if (Convert.ToInt32(playlistNode["votecount"].InnerText.Trim()) < 1)
                    {
                        playlistNode["score"].InnerText = score.ToString();
                    }
                    else
                    {
                        playlistNode["score"].InnerText = ((score +
                         Convert.ToDouble(playlistNode["score"].InnerText.Trim())) /
                         Convert.ToInt32(playlistNode["votecount"].InnerText.Trim())).ToString();
                    }

                    saveDataFile();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}//end class