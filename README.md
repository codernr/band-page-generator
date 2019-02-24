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
		* `Links`: key-value pairs of provider and album link, example: `{ "bandcamp": "https://band.bandcamp.com/album/some-album" }
* `General`
    * `DownloadedBasePath`: used only if `-d` command line option is used, image paths are appended to this, so it has to contain the url where the page is going to be hosted; example: `https://myband.com/downloads`
	
### How to run

The app can be run from the folder where it is downloaded with the above example values like this:

`dotnet BandPageGenerator.dll -o C:\BandPage\index.html -t C:\BandPage\Template -s C:\BandPage\settings.json -d C:\BandPage\downloads`
		
	