# Band page generator ![Gitlab pipeline status](https://img.shields.io/gitlab/pipeline/codernr/band-page-generator.svg) ![Codacy grade](https://img.shields.io/codacy/grade/5c7e61918adc4e0bbd5615b39e01998b.svg) ![Codecov](https://img.shields.io/codecov/c/gh/codernr/band-page-generator.svg)

A simple command line application to generate static HTML page for music groups with data acquired from their facebook, instagram and spotify profiles.

## Goal of this project

My primary goal is to create a software that is able to generate an up to date HTML page for my band based on recent activities from our social pages. The execution can be automated so this way the page can be updated regularly with new music, videos and gigs withous any manual interaction.

This project however doesn't intend to eliminate the work that must be done on the web design, so it uses [Handlebars.Net](https://github.com/rexm/Handlebars.Net) as a templating system for the processed HTML. In other words, user of this software has to sitebuild his own template.

You can see a working example here: [https://sz4p.hu](https://sz4p.hu)

## Installation

You can download the [latest release](https://github.com/codernr/band-page-generator/releases/latest) from the releases page of this repository.

The project is built with dotnet core 2.2 so it needs dotnet runtime to be run. [You can download it here](https://dotnet.microsoft.com/download)

Optionally, the app can be published to any platform as a standalone by dotnet SDK if you clone the repository.

## Usage

The app is configured with command line options and a settings JSON file.

### Command line options

* `-o`: The absolute path to the generated HTML page; example: `C:\BandPage\index.html`
* `-t`: The absolute path to the folder that contains the Handlebars template from which the output is generated. The template has to be named index.html (in a future release Handlebars partials can be loaded from here with other names)  
example: `C:\BandPage\Template`
* `-s`: The absolute path to the settings JSON file; example: `C:\BandPage\settings.json`
* `-d`: (optional) The absolute path to the folder where the image files (photos, cover images, etc.) will be downloaded. The images are served from the original facebook, instagram, youtube and spotify sources if not set); example: `C:\BandPage\downloads`

### Settings JSON file

**IMPORTANT**: The settings file includes sensitive API keys that should not be exposed publicly!

Example settings file can be found [here](https://github.com/codernr/band-page-generator/blob/develop/BandPageGenerator/appsettings.json)

* `Facebook`
    * `PageId`: unique facebook id of the band's facebook page, usually it is the unique url behind www.facebook.com/
    * `InstagramId`: unique id of the band's instagram account (must be linked with facebook page as instagram business page!)  
	you can find it with online tools [like this](http://www.allautoliker.com/instagram/find-id)
	* `FilterHashtags`: array of hashtags to filter instagram images (only images using any of these hashtags will be pusblished on the page)  
	example: `["#hashtag1", "#hashtag2]`
	* `AlbumId`: facebook graph id of a facebook photo album that is displayed on the page
	* `ApiVersion`: facebook graph api version to use (use `v3.2` as it is the most recent when writing this)
	* `AccessToken`: API access token to make requests to facebook API (documented in Wiki... TODO)
	* `PastEventDisplayLimit`: How many past events you want to display on the page
* `Youtube`
    * `ApiKey`: key to access Youtube data API (documeted in Wiki... TODO)
	* `ChannelId`: Id of the band's youtube channel
	* `AdditionalVideoIds`: an array of videos that are not published on the band's channel but is a band video (added to cumulated view count of the channel)
	* `FeaturedPlaylistId`: A youtube playlist from which videos are fetched to be played on the page
* `Spotify`
    * `ClientId`: spotify API client ID (documeted in Wiki... TODO)
	* `ClientSecret`: spotify API client secret (documeted in Wiki... TODO)
	* `ArtistId`: spotify artist ID
	* `AlternativeLinks`: an array of titled album link collection to other providers (like bandcamp, itunes, etc.)
	    * `Title`: the title of the album (albums are associated with the ones fetched from spotify API by title)
		* `Links`: key-value pairs of provider and album link, example: `{ "bandcamp": "https://band.bandcamp.com/album/some-album" }`
* `General`
    * `DownloadedBasePath`: used only if `-d` command line option is used, image paths are appended to this, so it has to contain the url where the page is going to be hosted; example: `https://myband.com/downloads`
	
### How to run

The app can be run from the folder where it is downloaded with the above example values like this:

`dotnet BandPageGenerator.dll -o C:\BandPage\index.html -t C:\BandPage\Template -s C:\BandPage\settings.json -d C:\BandPage\downloads`

## Getting API keys and tokens

You will need different API keys and tokens to access your social data with this app. These can be acquired differently from each provider.

### Facebook and Instagram

_Note: figuring this process out was almost as much work as to write this entire software. How ironic that it is nearly impossible to use graph api to access PUBLIC data from your own page while all the facebook partners are gathering the users' private data haha._

At the time of writing this, the Graph API is in v3.2 version. The only way to use it for accessing public data from your page is to create an API app and leave it in development mode.

Other prerequisites to make it work:

* you have to be the owner of the app
* you have to be admin on the facebook page you want to access
* your instagram account must be linked with your facebook page as a business account

Steps to acquire a never expiring access token to your page:

1. On [https://developers.facebook.com](https://developers.facebook.com) click on My apps, then Add new app
2. Give your app a name and create it
3. In Products + section of the dashboard, add Marketing API (click Set Up)
4. Go back to Products and add Instagram too
5. Go to Marketing API > Tools
6. In Get Access Token section check all the checkboxes and click "Get Token"
7. Copy the given token
8. Go to [Graph API Explorer](https://developers.facebook.com/tools/explorer/)
9. On the right select your Facebook App
10. Under permission type "instagram_basic" in the "Add a Permission" field
11. Click Get Access Token button on the bottom
12. Accept the popup
13. Above the Get token button paste the previously (7th step) copied token
14. Select your band's facebook page from the User or Page dropdown
15. Go back to Access Token field (where you pasted the previous token) and save its content, this is your final token
16. Check it on [Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken)

You should see something like this:

* type: Page
* Page ID: your band page ID and name
* Expires: Never

This token has to be set in the `Facebook > AccessToken` settings.

### Youtube

1. Go to [Google Developer Console](https://console.developers.google.com/) and sign in with your google account
2. Click on top left dropdown, then click create a project
3. Give it a name and create it
4. Go to Library on the left menu
5. Search for Youtube Data API v3
6. Select it and click on Enable
7. Go to Credentials on the left menu
8. Click + Create credential
9. For the *Where will you be calling the API from?* question select Other non-UI
10. From the radio buttons select public data
11. Click the button, save the key that is presented

This key should be set in the `Youtube > ApiKey` settings

### Spotify

1. Go to [Spotify developer dashboard](https://developer.spotify.com/dashboard/)
2. Log in to Spotify
3. Click create client id
4. Fill in the form, select Desktop app checkbox
5. Click 'No' on the next page (_Are you developing a commercial integration?_)
6. Check all the checkboxes and continue
7. Copy your client ID to settings JSON: `Spotify > ClientId`
8. Click 'Show client secret' and copy the value to settings JSON: `Spotify > ClientSecret`

## Templating

In the provided template index.html one can use Handlebars syntax to display gathered data from provider APIs. For full reference of the syntax, visit [https://handlebarsjs.com/](https://handlebarsjs.com/)

An example template is included in this project, you can browse it [here](https://github.com/codernr/band-page-generator/blob/develop/BandPageGenerator/Templates/index.html)

### Template model hierarchy

#### Main template object

* `Likes`: Facebook page like count
* `ProfilePictureUrl`: Url of facebook page profile picture
* `UpcomingEvents`: Page events in the future, array of [FacebookEventModel](#facebook-event-model)
* `PastEvents`: Past page events limited by `PastEventDisplayLimit` count, array of [FacebookEventModel](#facebookeventmodel)
* `FeaturedPhotos`: Featured facebook photo album (specified by album ID in facebook [settings](#settings-json-file)), array of [FacebookPhotoModel](#facebookphotomodel)
* `MemberPhotos`: Member facebook photo album (_note that captions of member photos on facebook have to have a line break in them, the first line is member name, the last is description_), array of [FacebookMemberPhotoModel](#facebookmemberphotomodel)
* `InstagramPhotos`: Recent instagram photos, array of [FacebookInstagramMediaModel](#facebookinstagrammediamodel)
* `ViewCount`: Cumulated view count of all videos of the band on Youtube
* `Videos`: Videos of featured playlist on Youtube (specified by `FeaturedPlaylistId` in [settings](#settings-json-file)), array of [YoutubeVideoModel](#youtubevideomodel)
* `Albums`: Albums of band on Spotify, array of [SpotifyAlbumTemplateModel](#spotifyalbumtemplatemodel)

#### FacebookEventModel

#### FacebookPhotoModel

#### FacebookMemberPhotoModel

#### FacebookInstagramMediaModel

#### YoutubeVideoModel

#### SpotifyAlbumTemplateModel
