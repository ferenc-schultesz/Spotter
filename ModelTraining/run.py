from __future__ import absolute_import
from __future__ import division
from __future__ import print_function

import retrain as train
import os
import threading


def initModelOutputFolders(model_name, image_dir):
    train.image_dir = image_dir
    train.output_graph = '../Outputs/' + model_name + '/output_graph.pb'
    train.intermediate_output_graphs_dir = '../Outputs/' + model_name + '/intermediate/'
    train.output_labels = '../Outputs/' + model_name + '/output_labels.txt'
    train.summaries_dir = '../Outputs/' + model_name + '/summaries/'
    train.bottleneck_dir = '../Outputs/' + model_name + '/bottleneck_values/'
    train.saved_model_dir = '../Outputs/' + model_name + '/Exported_graph/'
    train.model_name = model_name
    return


def logDetailsOfModel():
    if not os.path.exists('../Outputs/' + model_name + '/'):
        os.makedirs('../Outputs/' + model_name + '/')
    file = open('../Outputs/' + model_name + '/hyperparameters.txt', 'w')
    file.write('----------------------------------------------------' + '\n')
    file.write('----------------  HYPERPARAMETERS  -----------------' + '\n')
    file.write('----------------------------------------------------' + '\n\n')
    file.write('image_dir = ' + str(train.image_dir) + '\n')
    file.write('output_graph = ' + str(train.output_graph) + '\n')
    file.write('intermediate_output_graphs_dir = ' + str(train.intermediate_output_graphs_dir) + '\n')
    file.write('output_labels = ' + str(train.output_labels) + '\n')
    file.write('summaries_dir = ' + str(train.summaries_dir) + '\n')
    file.write('bottleneck_dir = ' + str(train.bottleneck_dir) + '\n')
    file.write('how_many_training_steps = ' + str(train.how_many_training_steps) + '\n')
    file.write('learning_rate = ' + str(train.learning_rate) + '\n')
    file.write('train_batch_size = ' + str(train.train_batch_size) + '\n')
    file.write('random_crop = ' + str(train.random_crop) + '\n')
    file.write('random_scale = ' + str(train.random_scale) + '\n')
    file.write('random_brightness = ' + str(train.random_brightness) + '\n')
    file.close()
    return


# # test
# train.how_many_training_steps = 5
# train.learning_rate = 0.01
# train.train_batch_size = 10
# train.random_crop = 0
# train.random_scale = 10
# train.random_brightness = 15
# train.flip_left_right = True

# model_name = 'test_model_1'
# initModelOutputFolders(model_name, '../Test/')
# logDetailsOfModel()
# train.train_model()

# # test
# train.how_many_training_steps = 10
# train.learning_rate = 0.01
# train.train_batch_size = 20
# train.random_crop = 0
# train.random_scale = 10
# train.random_brightness = 15
# train.flip_left_right = True

# model_name = 'test_model_2'
# initModelOutputFolders(model_name, '../Test/')
# logDetailsOfModel()
# train.train_model()

# # test
# train.how_many_training_steps = 15
# train.learning_rate = 0.01
# train.train_batch_size = 30
# train.random_crop = 0
# train.random_scale = 10
# train.random_brightness = 15
# train.flip_left_right = True

# model_name = 'test_model_3'
# initModelOutputFolders(model_name, '../Test/')
# logDetailsOfModel()
# train.train_model()

#------------------------------------

# melanoma_nevus_m1
train.how_many_training_steps = 400
train.learning_rate = 0.01
train.train_batch_size = 1000
train.random_crop = 0
train.random_scale = 10
train.random_brightness = 15

model_name = 'melanoma_nevus_m1'
initModelOutputFolders(model_name, '../TrainingData/')
logDetailsOfModel()
train.train_model()

# #------------------------------------

# melanoma_nevus_m2
train.how_many_training_steps = 200
train.learning_rate = 0.01
train.train_batch_size = 1500
train.random_crop = 0
train.random_scale = 10
train.random_brightness = 15

model_name = 'melanoma_nevus_m2'
initModelOutputFolders(model_name, '../TrainingData/')
logDetailsOfModel()
train.train_model()

#------------------------------------

# melanoma_nevus_m3
train.how_many_training_steps = 200
train.learning_rate = 0.02
train.train_batch_size = 1500
train.random_crop = 0
train.random_scale = 10
train.random_brightness = 15

model_name = 'melanoma_nevus_m3'
initModelOutputFolders(model_name, '../TrainingData/')
logDetailsOfModel()
train.train_model()

#------------------------------------

# melanoma_nevus_m4
train.how_many_training_steps = 200
train.learning_rate = 0.01
train.train_batch_size = 2500
train.random_crop = 0
train.random_scale = 10
train.random_brightness = 15

model_name = 'melanoma_nevus_m4'
initModelOutputFolders(model_name, '../TrainingData/')
logDetailsOfModel()
train.train_model()
