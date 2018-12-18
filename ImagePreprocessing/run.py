import cv2
import pyodbc
from shutil import copyfile

# Path for all data sets
DATA_PATH = "d:/Data/"
DATA_PATH_TRAINING = "d:/TrainingData/"

conn = pyodbc.connect("Driver={SQL Server Native Client 11.0};"
                      "Server=discpc\LOCALDB;"
                      "Database=isic;"
                      "Trusted_Connection=Yes"
                      )

cursor = conn.cursor()
# RESIZING AND MOVING DATA TO TRAINING FOLDER
# Get list of images from DB
query = """select [name], [meta_clinical_diagnosis], [dataset_name] from [isic].[dbo].[ImageData] where [meta_clinical_diagnosis] in ('melanoma','nevus')"""
cursor.execute(query)

# Choose particular images, reduce sizes and copy to new folders
desired_size = 299
count = 0
for row in cursor:
    count = count + 1
    fname = DATA_PATH + row[0] + '.jpg' # build filename
    img = cv2.imread(fname) # read image to variable
    # build output path for nevus or for melanoma image
    if row[1] == 'nevus':
        proc_fname = DATA_PATH_TRAINING + 'nevus/' + row[0] + '.jpg'
    if row[1] == 'melanoma':
        proc_fname = DATA_PATH_TRAINING + 'melanoma/' + row[0] + '.jpg'
    (h, w) = img.shape[:2] # get image resolution
    # take smaller dimension as the new size (square)
    if h < w:
        size = h
    else:
        size = w
    # crop the image, using the "new size" for both height and width
    cropped_img = img[int(h/2-size/2):int(h/2+size/2),int(w/2-size/2):int(w/2+size/2)]
    # resize the image to 299x299
    resized_img = cv2.resize(cropped_img, (299, 299))
    # save the file to the new path
    cv2.imwrite(proc_fname, resized_img)
    # print the number of images processed after every 100 images
    if count % 100 == 0:
        print(count)
