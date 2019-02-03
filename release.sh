#!/bin/bash

body='{"tag_name": "%s", "name":"%s","body":"Automatic release", "draft": true}'

printf -v formatted_body "$body" "$CI_COMMIT_TAG" "$CI_COMMIT_TAG"

upload_url=$(curl -s -H "Authorization: token $GITHUB_RELEASE_TOKEN" -d "$formatted_body" "https://api.github.com/repos/codernr/band-page-generator/releases")

upload_url=$(egrep -oh -m 1 "(https://uploads.github.com/repos/codernr/band-page-generator/releases/[0-9]+/assets)" <<< $upload_url)

echo "uploading asset to release to url : $upload_url"
curl -s -H "Authorization: token $GITHUB_RELEASE_TOKEN" -H "Content-Type: application/tar+gzip" --data-binary @$CI_COMMIT_TAG.tar.gz "$upload_url?name=$CI_COMMIT_TAG.tar.gz"
