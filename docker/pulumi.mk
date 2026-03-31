# Copyright 2026 dah4k
# SPDX-License-Identifier: EPL-2.0

pulumi:         ## Build and run Pulumi devcontainer
pulumi-debug:   ## Debug current Pulumi devcontainer as Root
pulumi-clean:   ## Stop and remove Pulumi devcontainer

DOCKER      ?= docker
REGISTRY    ?= local
IMAGE       := pulumi
TAG         := $(REGISTRY)/$(IMAGE)
DOCKERFILE  := docker/Dockerfile.$(IMAGE)
VOLUMES     := --volume .:/home/devops/src/:Z

pulumi: $(DOCKERFILE)
	$(DOCKER) build --tag $(TAG) --file $< .
	$(DOCKER) run --interactive --tty --rm --name=$(IMAGE) $(VOLUMES) $(TAG)

pulumi-debug:
	$(DOCKER) exec --interactive --tty --user root `$(DOCKER) ps --filter name=$(IMAGE) --quiet` /bin/bash

pulumi-clean:
	$(DOCKER) stop $(IMAGE) || true
	$(DOCKER) image remove --force $(TAG)

.PHONY: pulumi pulumi-run pulumi-debug pulumi-clean
