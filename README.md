# Band page generator ![Gitlab pipeline status](https://img.shields.io/gitlab/pipeline/codernr/band-page-generator.svg) ![Codacy grade](https://img.shields.io/codacy/grade/5c7e61918adc4e0bbd5615b39e01998b.svg) ![Codecov](https://img.shields.io/codecov/c/gh/codernr/band-page-generator.svg)

A simple command line application to generate static HTML page for music groups with data acquired from their facebook, instagram and spotify profiles.

## Goal of this project

My primary goal is to create a software that is able to generate an up to date HTML page for my band based on recent activities from our social pages. The execution can be automated so this way the page can be updated regularly with new music, videos and gigs withous any manual interaction.

This project however doesn't intend to eliminate the work that must be done on the web design, so it uses [Handlebars.Net](https://github.com/rexm/Handlebars.Net) as a templating system for the processed HTML. In other words, user of this software has to sitebuild his own template.

## Installation

You can download the [latest release](https://github.com/codernr/band-page-generator/releases/latest) from the releases page of this repository.

The project is built with dotnet core 2.2 so it needs dotnet runtime to be run. [You can download it here](https://dotnet.microsoft.com/download)

Optionally, the app can be published to any platform as a standalone by dotnet SDK if you clone the repository.

