import os, sys, shutil


cwd = os.getcwd()
path_unity_models = cwd + "/../NRP"

default_path_models_repo = "D:/workspaces/nrp/Models"


def copy_folder(path):
    destination_dir = path_unity_models + "/" + os.path.basename(path)
    if not os.path.exists(destination_dir):
        shutil.copytree(path, destination_dir)


def copy_all_model_folders(path):
    for root, dirs, files in os.walk(path):
        if "model.config" in files \
                or "model" in dirs \
                or "model_library.json" in files:
            copy_folder(root)


def main():
    if len(sys.argv) < 2:
        print("no models repository path specified as argument! using default " + default_path_models_repo)
        path_models_repo = default_path_models_repo
    else:
        path_models_repo = sys.argv[1]

    if not os.path.exists(path_models_repo):
        print("specified models repository path does not exist! aborting")
        return

    if not os.path.exists(path_unity_models):
        os.makedirs(path_unity_models)

    copy_all_model_folders(path_models_repo)


if __name__ == "__main__":
    main()
