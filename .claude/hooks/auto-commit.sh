#!/usr/bin/env bash
# auto-commit.sh
# Stop 이벤트에서 변경사항 자동 커밋 + origin push
# - 변경사항 없으면 skip
# - "auto:" prefix로 식별
# - 커밋 메시지는 변경 파일 분석으로 의미 있는 제목 생성
# - 커밋 성공 후 origin/<현재 브랜치> 로 push (origin remote 있는 경우만)
# - 네트워크 실패해도 후크는 성공 종료 (다음 사이클에서 재시도)

set -euo pipefail

REPO_PATH="C:/Users/jayju/project/Cultivation"
cd "$REPO_PATH" || exit 0

# 변경 없으면 종료
CHANGES=$(git status --porcelain | wc -l)
if [ "$CHANGES" -eq 0 ]; then
    exit 0
fi

# 변경 파일 목록 (모두)
CHANGED_PATHS=$(git status --porcelain | awk '{print $2}')

# === 제목 생성 로직 ===
# 1) 단일 파일: basename 사용
# 2) 모두 같은 상위 폴더(2~3단계): "<폴더> — N files"
# 3) 다영역: 가장 많이 변경된 영역 + N files

count_files() { echo "$1" | grep -c .; }

basename_first() { echo "$CHANGED_PATHS" | head -1 | xargs -I{} basename {}; }

# 공통 prefix 디렉터리 찾기 (가장 깊은 공통 부모 — 마지막 path segment 기준)
common_dir() {
    local first
    first=$(echo "$CHANGED_PATHS" | head -1)
    local prefix="$first"
    while [ -n "$prefix" ]; do
        if echo "$CHANGED_PATHS" | awk -v p="$prefix/" '{ if (index($0, p) != 1 && $0 != substr(p,1,length(p)-1)) exit 1 }'; then
            # prefix 가 모든 파일의 공통 시작이면 부모 디렉터리 반환
            echo "$prefix"
            return
        fi
        prefix=$(dirname "$prefix")
        [ "$prefix" = "." ] && break
    done
    echo ""
}

# 가장 많이 등장하는 디렉터리 (1단계 깊이)
top_area() {
    echo "$CHANGED_PATHS" | awk -F/ '{print $1"/"$2}' | sort | uniq -c | sort -rn | head -1 | awk '{$1=""; sub(/^ /,""); print}'
}

if [ "$CHANGES" -eq 1 ]; then
    SUBJECT="auto: $(basename_first)"
else
    # 각 path의 dirname 추출 (awk: 마지막 / 이전까지)
    DIRS=$(echo "$CHANGED_PATHS" | awk -F/ 'NF>1 { OFS="/"; NF--; print } NF==1 { print "." }')
    UNIQUE_DIRS=$(echo "$DIRS" | sort -u | wc -l)
    if [ "$UNIQUE_DIRS" -eq 1 ]; then
        DIR=$(echo "$DIRS" | awk 'NR==1{print; exit}')
        SHORT_DIR=$(echo "$DIR" | awk -F/ '{ if (NF>=2) print $(NF-1)"/"$NF; else print $0 }')
        SUBJECT="auto: ${SHORT_DIR} — ${CHANGES} files"
    else
        AREA=$(echo "$CHANGED_PATHS" | awk -F/ '{print $1"/"$2}' | sort | uniq -c | sort -rn | awk 'NR==1{$1=""; sub(/^ /,""); print; exit}')
        SUBJECT="auto: ${AREA} +others — ${CHANGES} files"
    fi
fi

TIMESTAMP=$(date +"%Y-%m-%d %H:%M")

# 본문: 변경 파일 목록 (최대 15개)
BODY_FILES=$(echo "$CHANGED_PATHS" | head -15 | sed 's/^/- /')
EXTRA=$((CHANGES - 15))
if [ "$EXTRA" -gt 0 ]; then
    BODY_FILES="${BODY_FILES}
- (+${EXTRA} more)"
fi

git add -A

git commit -m "${SUBJECT}

Changed at ${TIMESTAMP}:
${BODY_FILES}

[Auto-committed by Stop hook]" 2>&1 | tail -3

if git remote get-url origin >/dev/null 2>&1; then
    BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "")
    if [ -n "$BRANCH" ] && [ "$BRANCH" != "HEAD" ]; then
        echo "[auto-commit] pushing to origin/${BRANCH}..."
        git push origin "$BRANCH" 2>&1 | tail -3 || echo "[auto-commit] push 실패 — 다음 사이클에서 재시도"
    fi
fi

exit 0
