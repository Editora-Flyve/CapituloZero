---
title: CI/CD
description: Fluxo de PR, qualidade e deploy com GitHub Actions
---

# CI/CD

Este projeto usa GitHub Actions com os seguintes pipelines:

- PR (CI): build .NET 9, testes, cobertura (Cobertura), verificação de cobertura mínima de 90% para novo código (patch) e fallback de cobertura total. Opcionalmente, integra com SonarCloud e Codecov se configurados.
- Main (CI/CD): build, testes, cobertura para todo o código, quality gate no SonarCloud (opcional) e job de deploy (placeholder).
- CodeQL: varredura de segurança em PR, main e agendado semanalmente.

## Configuração opcional

Crie os seguintes itens no repositório para habilitar integrações:

- Secrets
  - SONAR_TOKEN: token de projeto/organização do SonarCloud.
  - CODECOV_TOKEN: token do Codecov (se o repositório não for público ou for exigido).
- Variables
  - SONAR_PROJECT_KEY: chave do projeto no SonarCloud (org/proj ou apenas proj).
  - SONAR_ORGANIZATION: organização no SonarCloud.

Arquivo `codecov.yml` já foi adicionado para impor 90% em project e patch.

## Deploy

O job de deploy atualmente é um placeholder. Substitua-o por um dos exemplos:

- Docker push + GitHub Container Registry
- Azure Web App for Containers
- Kubernetes (kubectl/helm)

Adapte permissões de ambiente e secrets conforme o provedor escolhido.
