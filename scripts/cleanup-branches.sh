#!/bin/bash

# LMJ : Manually clean up local branches that have been deleted on remote

echo "🧹 Cleaning up stale local branches..."
echo ""

# LMJ : Prune remote tracking branches
echo "📡 Fetching remote changes..."
git fetch --prune

echo ""
echo "🔍 Finding stale branches..."
echo ""

# LMJ : Get list of branches that are gone on remote
STALE_BRANCHES=$(git branch -vv | grep ': gone]' | awk '{print $1}')

if [ -z "$STALE_BRANCHES" ]; then
    echo "✅ No stale branches found. Everything is clean!"
    exit 0
fi

echo "Found the following stale branches:"
echo "$STALE_BRANCHES" | while read branch; do
    echo "  ❌ $branch"
done

echo ""
read -p "❓ Delete these branches? (y/N): " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    DELETED_COUNT=0
    echo "$STALE_BRANCHES" | while read branch; do
        if git branch -D "$branch" 2>/dev/null; then
            echo "  ✅ Deleted: $branch"
            DELETED_COUNT=$((DELETED_COUNT + 1))
        else
            echo "  ⚠️  Failed to delete: $branch"
        fi
    done
    echo ""
    echo "🎉 Cleanup completed!"
else
    echo "❌ Cleanup cancelled."
fi
