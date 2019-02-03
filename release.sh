#!/bin/bash

GITHUB_RELEASE_TOKEN=b496b41a5bf7f0b87e9c6803f9255e9cb133728c
CI_COMMIT_TAG=terst-ci-deploy-6

upload_url=$(curl -s -H "Authorization: token $GITHUB_RELEASE_TOKEN" -d '{"tag_name": "$CI_COMMIT_TAG", "name":"$CI_COMMIT_TAG","body":"Automatic release", "draft": true}' "https://api.github.com/repos/codernr/band-page-generator/releases")

upload_url=$(egrep -oh -m 1 "(https://uploads.github.com/repos/codernr/band-page-generator/releases/[0-9]+/assets)" <<< $upload_url)

echo "uploading asset to release to url : $upload_url"
curl -s -H "Authorization: token $GITHUB_RELEASE_TOKEN" -H "Content-Type: application/tar+gzip" --data-binary @$CI_COMMIT_TAG.tar.gz "$upload_url?name=$CI_COMMIT_TAG.tar.gz"
