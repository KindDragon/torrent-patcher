Torrent Patcher
by ArielTM (arieltm@gmail.com, projects@arieltm.net) & Vedmed

A Little program to load .torrent files and decrypt them from their bencoded form.

Please note the program needs the Microsoft .NET Framework v2.0
http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5 (for x86)
Features:

    * Class properties to get the most common data from the torrent file
    * Both Normal view and a tree view, preserving the structural form of the torrent file
    * A Debug timer to measure the overall time to load the torrent
    * Useful links
    * A thoroughly commented code (Mostly in the Torrent Parser class)

Notes:

    * I used a lot of casting because of the four (or even five) different types of data in the Bencoding, making the code a bit complicated. Though I have written other functions to compensate on this disadvantage.
    * It is advised to to understand the code better, but this is not obligatory

Useful Links:

    * http://wiki.theory.org/BitTorrentSpecificatin - Bittorrent Protocol Specification v1.0
    * http://www.bittorrent.org/protocol.html - BitTorrent.org For Developers
    * http://torrentspy.sourceforge.net/ - A program to do show all the available information in a torrent
    * http://pscode.com/vb/scripts/ShowCode.asp?txtCodeId=3360&lngWId=10 - A torrent parser class written in VB.Net

