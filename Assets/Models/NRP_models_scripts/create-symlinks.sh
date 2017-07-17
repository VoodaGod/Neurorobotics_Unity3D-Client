#!/bin/bash
# Author: Bernd Eckstein
#
# This script reads models from _rpmbuild/models.txt
# and creates symlinks in ~/.gazebo/models
# It flattens models located in subdirectories

function log() {
    echo "[HBP-NRP] $(date --rfc-3339=seconds) $*"
}

function create_symlinks() {
    log "Creating symlinks to ~/.gazebo/models"
    local DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
    local MODELS=$(cat "$DIR/_rpmbuild/models.txt")
    for model in ${MODELS[@]}; do
        linkname=~/.gazebo/models/$(basename $model)
        if [ ! -d $linkname ]; then
            log "Creating link for $model"
            ln -s $DIR/$model $linkname
        else
            log "Link for $model already exists"
        fi
    done
}

create_symlinks
