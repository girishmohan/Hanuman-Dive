#!/bin/bash
# Publishes the browser build (WebBuild/) to the gh-pages branch, which GitHub Pages
# serves at https://girishmohan.github.io/Hanuman-Dive/
#
#   1. Build:   Unity > Tools > Hanuman Dive > Build WebGL   (writes WebBuild/)
#   2. Publish: tools/publish_web.sh
set -euo pipefail

repo="$(cd "$(dirname "$0")/.." && pwd)"
build="$repo/WebBuild"
[ -f "$build/index.html" ] || { echo "No WebBuild/index.html: build WebGL in Unity first."; exit 1; }

work="$(mktemp -d)"
trap 'git -C "$repo" worktree remove --force "$work" 2>/dev/null || true; rm -rf "$work"' EXIT

git -C "$repo" fetch -q origin gh-pages 2>/dev/null || true
if git -C "$repo" rev-parse -q --verify origin/gh-pages >/dev/null; then
  git -C "$repo" worktree add -q -B gh-pages "$work" origin/gh-pages
else
  git -C "$repo" worktree add -q --detach "$work"
  git -C "$work" checkout -q --orphan gh-pages
fi

# Replace the site contents with the fresh build.
find "$work" -mindepth 1 -maxdepth 1 ! -name .git -exec rm -rf {} +
cp -R "$build"/. "$work"/
touch "$work/.nojekyll"   # serve files as-is (no Jekyll processing)

git -C "$work" add -A
if git -C "$work" diff --cached --quiet; then
  echo "Nothing changed since the last publish."
  exit 0
fi
git -C "$work" commit -q -m "Publish web build $(date '+%Y-%m-%d %H:%M')"
git -C "$work" push -q origin gh-pages
echo "Published. Live in a minute or two at https://girishmohan.github.io/Hanuman-Dive/"
