# Copyright 2026 dah4k
# SPDX-License-Identifier: EPL-2.0

DOCKER      ?= docker
REGISTRY    ?= local
IMAGES      := $(subst Dockerfile.,,$(wildcard Dockerfile.*))
TAGS        := $(addprefix $(REGISTRY)/,$(IMAGES))
TEST_IMAGE  := pulumi-devcontainer
_ANSI_NORM  := \033[0m
_ANSI_CYAN  := \033[36m

.PHONY: help usage
help usage:
	@grep -hE '^[0-9a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?##"}; {printf "$(_ANSI_CYAN)%-20s$(_ANSI_NORM) %s\n", $$1, $$2}'

.PHONY: all
all: $(TAGS) ## Build all container images

$(REGISTRY)/%: Dockerfile.%
	$(DOCKER) build --tag $@ --file $< .

.PHONY: test
test: $(REGISTRY)/$(TEST_IMAGE) ## Test runtime container image
	$(DOCKER) run --interactive --tty --rm --name=$(TEST_IMAGE) $<

.PHONY: debug
debug: ## Debug last run container image
	$(DOCKER) exec --interactive --tty --user root `docker ps --latest --quiet` /bin/bash

.PHONY: clean
clean: ## Remove all container images
	$(DOCKER) image remove --force $(TAGS)

.PHONY: distclean
distclean: clean ## Prune all container images
	$(DOCKER) image prune --force
	$(DOCKER) system prune --force
