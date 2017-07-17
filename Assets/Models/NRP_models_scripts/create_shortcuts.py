#from win32com.client import Dispatch  # requires pip install pypiwin32
#import winshell  # requires pip install winshell
#import os
import win32file


#def create_shortcut(target_path, shortcut_path, startin, icon_path = None):
#
#    shell = Dispatch('WScript.Shell')
#    shortcut = shell.CreateShortCut(shortcut_path)
#    if shortcut:
#        print("target path: " + target_path)
#        print("shortcut path: " + shortcut_path)
#        windows_target_path = target_path.replace("/", "\\")
#        print("windows shortcut path: " + windows_target_path)
#        #shortcut.Targetpath = windows_target_path
#        shortcut.WorkingDirectory = startin
#        if icon_path:
#            shortcut.IconLocation = icon_path
#        shortcut.save()
#    else:
#        print("creating shortcut for \"" + target_path + "\" failed")


### TRY REMOVING "_" for shortcuts ! adjust in unity as well ###

path_nrp_models_folder = "G:/workspaces/hbp/nrp/Models"
path_unity_client_nrp_models = "G:/workspaces/hbp/nrp/testfolder"

file_models_list = path_nrp_models_folder + "/_rpmbuild/models.txt"
f = open(file_models_list)
for model_subpath in f:
    model_name = model_subpath.split("/")[-1]
    model_path = path_nrp_models_folder + "/" + model_subpath
    shortcut_path = path_unity_client_nrp_models + "/" + model_name
    print("model path: " + model_path)
    print("shortcut path: " + shortcut_path)
    win32file.CreateSymbolicLink(shortcut_path, model_path, 1)
f.close()

#path_target = "C:/Users/sandman/Documents"
#path_shortcut = "C:/Users/sandman/Desktop/docs"
#win32file.CreateSymbolicLink(path_shortcut, path_target, 1)