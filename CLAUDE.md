# Claude Code Game Studios -- Game Studio Agent Architecture

Indie game development managed through 48 coordinated Claude Code subagents.
Each agent owns a specific domain, enforcing separation of concerns and quality.

## Technology Stack

- **Engine**: Unity 6.3 LTS (6000.3.x)
- **Language**: C#
- **Version Control**: Git with trunk-based development
- **Build System**: Unity Build Pipeline (Build Profiles)
- **Asset Pipeline**: Unity Asset Import Pipeline + Addressables

> **Note**: This project uses **Unity 6.3 LTS** exclusively.
> Unity-specific agents (`unity-csharp`, `unity-architect`, `unity-debugger`) are active.
> Godot agents are disabled in `.claude/agents/_disabled/`. Unreal agents remain
> available but should not be invoked unless the project pivots.

## Project Path

- **Unity Project Root**: `src/Cultivation/`
- **Unity Version**: 6000.3.10f1 (Unity 6.3)
- **Render Pipeline**: Universal Render Pipeline (URP)

## Project Structure

@.claude/docs/directory-structure.md

## Engine Version Reference

@docs/engine-reference/unity/VERSION.md

## Technical Preferences

@.claude/docs/technical-preferences.md

## Coordination Rules

@.claude/docs/coordination-rules.md

## Collaboration Protocol

**User-driven collaboration, not autonomous execution.**
Every task follows: **Question -> Options -> Decision -> Draft -> Approval**

- Agents MUST ask "May I write this to [filepath]?" before using Write/Edit tools
- Agents MUST show drafts or summaries before requesting approval
- Multi-file changes require explicit approval for the full changeset
- No commits without user instruction

See `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` for full protocol and examples.

> **First session?** If the project has no engine configured and no game concept,
> run `/start` to begin the guided onboarding flow.

## Workflow

**B타입 워크플로우 (2026-04-26~)**: 1인 개발자(PM/결정자) + AI(풀스택 개발자) 협업 모델.
Sprint 시스템 폐지, continuous flow.

@.claude/docs/workflow-b-type.md

@.claude/docs/editor-handoff.md

@.claude/docs/review-workflow.md

## Deprecated Concepts (참고 금지)

초기 기획에서 폐기된 컨셉이 코드/문서에 잔재할 수 있다.
새 작업 전 반드시 확인:

@.claude/docs/deprecated-concepts.md

## Naming Conventions

@.claude/docs/naming-conventions.md

## Coding Standards

@.claude/docs/coding-standards.md

## Context Management

@.claude/docs/context-management.md

## Working Principles

보리스 체르니(Claude Code 창시자) 룰에서 본 프로젝트(B타입)에 정합한 항목만 채택.

- **Simplicity / No Laziness / Minimal Impact**: 가능한 가장 단순한 변경. 근본 원인까지 추적하고 임시방편 금지(시니어 개발자 기준). 변경은 꼭 필요한 코드에만 닿게 — 새 버그 도입 회피.
- **Verify Before Done**: "동작함"을 증명하기 전 완료 표시 금지. 자문: "시니어 엔지니어가 이걸 통과시킬까?" — 테스트 실행, 로그 확인, main과 diff 비교 등 증명 수단을 명시. UI 변경은 스크린샷/Play 테스트로 본인 검증까지 도달해야 완료(B타입: AI는 Editor 핸드오프 + 자가 검증 체크리스트 제시).
- **Demand Elegance (사소하지 않은 변경 한정)**: 수정이 hacky하게 느껴지면 멈추고 "지금 알게 된 걸 다 안다고 치고 우아한 해법을 다시 짜라"를 자문. 단순 수정엔 적용하지 말 것(오버엔지니어링 방지).
- **Lessons Loop**: 사용자 교정을 받으면 **즉시** auto memory(`feedback` 타입)에 패턴 + Why + How to apply 기록. 같은 실수 재발 시 기존 memory를 갱신해 강화. 세션 시작 시 관련 memory 우선 검토.
