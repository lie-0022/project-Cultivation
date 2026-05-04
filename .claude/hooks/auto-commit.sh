#!/usr/bin/env bash
# auto-commit.sh
# Stop 이벤트에서 변경사항 자동 커밋 + origin push
# - 변경사항 없으면 skip
# - "auto:" prefix로 식별
# - 커밋 성공 후 origin/<현재 브랜치> 로 push (origin remote 있는 경우만)
# - 네트워크 실패해도 후크는 성공 종료 (다음 사이클에서 재시도)

set -euo pipefail

# 메인 워크트리 경로
REPO_PATH="C:/Users/jayju/project/Cultivation"

# 변경사항 확인
cd "$REPO_PATH" || exit 0

# untracked + modified 모두 체크
CHANGES=$(git status --porcelain | wc -l)

if [ "$CHANGES" -eq 0 ]; then
    # 변경 없음, 조용히 종료
    exit 0
fi

# 변경된 파일 목록 (최대 5개만 메시지에 포함)
CHANGED_FILES=$(git status --porcelain | head -5 | awk '{print $2}' | tr '\n' ',' | sed 's/,$//')
if [ "$CHANGES" -gt 5 ]; then
    CHANGED_FILES="${CHANGED_FILES} (+$(($CHANGES - 5)) more)"
fi

# 타임스탬프
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")

# 모든 변경 add
git add -A

# 커밋
git commit -m "auto: session checkpoint at ${TIMESTAMP}

Files changed: ${CHANGED_FILES}

[Auto-committed by Stop hook]" 2>&1 | tail -3

# 커밋 성공 시 origin remote가 있으면 push 시도 (실패해도 무시)
if git remote get-url origin >/dev/null 2>&1; then
    BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "")
    if [ -n "$BRANCH" ] && [ "$BRANCH" != "HEAD" ]; then
        echo "[auto-commit] pushing to origin/${BRANCH}..."
        git push origin "$BRANCH" 2>&1 | tail -3 || echo "[auto-commit] push 실패 (네트워크/인증) — 다음 사이클에서 재시도"
    fi
fi

exit 0
